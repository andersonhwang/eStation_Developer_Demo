import struct


def _read_exact(file_obj, size, error_message):
    data = file_obj.read(size)
    if len(data) != size:
        raise ValueError(error_message)
    return data


def _normalize_mask(mask):
    if mask == 0:
        return None

    shift = 0
    temp = mask
    while (temp & 1) == 0:
        temp >>= 1
        shift += 1

    bits = 0
    while (temp & 1) == 1:
        temp >>= 1
        bits += 1

    max_value = (1 << bits) - 1 if bits > 0 else 0
    return shift, bits, max_value


def _extract_masked_component(pixel_value, mask_info, default_value=0):
    if mask_info is None:
        return default_value

    shift, bits, max_value = mask_info
    value = (pixel_value >> shift) & max_value

    if max_value == 0:
        return default_value

    return (value * 255 + (max_value // 2)) // max_value


def _read_bitfields_masks(bmp_file, dib_header, dib_header_size, data_offset, bits_per_pixel):
    red_mask = green_mask = blue_mask = alpha_mask = 0

    # BITMAPV2+/V4+/V5 headers can embed masks directly in the DIB header.
    if dib_header_size >= 52:
        red_mask, green_mask, blue_mask = struct.unpack('<III', dib_header[40:52])

    if dib_header_size >= 56:
        alpha_mask = struct.unpack('<I', dib_header[52:56])[0]

    if red_mask or green_mask or blue_mask:
        return {
            'red': _normalize_mask(red_mask),
            'green': _normalize_mask(green_mask),
            'blue': _normalize_mask(blue_mask),
            'alpha': _normalize_mask(alpha_mask),
        }

    # BITMAPINFOHEADER + BI_BITFIELDS stores masks in the bytes between the
    # DIB header and pixel array.
    current_pos = bmp_file.tell()
    mask_bytes_available = data_offset - current_pos
    required_mask_bytes = 12

    if mask_bytes_available < required_mask_bytes:
        raise ValueError(
            f'BMP bitfields masks are incomplete for {bits_per_pixel}-bit image'
        )

    mask_data = _read_exact(
        bmp_file,
        required_mask_bytes,
        'BMP bitfields masks are incomplete'
    )
    red_mask, green_mask, blue_mask = struct.unpack('<III', mask_data)

    remaining_before_pixels = data_offset - bmp_file.tell()
    if remaining_before_pixels >= 4:
        alpha_mask = struct.unpack(
            '<I',
            _read_exact(bmp_file, 4, 'BMP alpha mask is incomplete')
        )[0]

    return {
        'red': _normalize_mask(red_mask),
        'green': _normalize_mask(green_mask),
        'blue': _normalize_mask(blue_mask),
        'alpha': _normalize_mask(alpha_mask),
    }


def _decode_rgb_pixel(row_data, pixel_offset, bits_per_pixel, compression, masks):
    if bits_per_pixel == 24:
        blue = row_data[pixel_offset]
        green = row_data[pixel_offset + 1]
        red = row_data[pixel_offset + 2]
        alpha = 255
        return blue, green, red, alpha

    if bits_per_pixel == 32:
        pixel_value = struct.unpack_from('<I', row_data, pixel_offset)[0]

        if compression == 0:
            blue = row_data[pixel_offset]
            green = row_data[pixel_offset + 1]
            red = row_data[pixel_offset + 2]
            alpha = row_data[pixel_offset + 3]
            return blue, green, red, alpha

        red = _extract_masked_component(pixel_value, masks['red'])
        green = _extract_masked_component(pixel_value, masks['green'])
        blue = _extract_masked_component(pixel_value, masks['blue'])
        alpha = _extract_masked_component(pixel_value, masks['alpha'], 255)
        return blue, green, red, alpha

    if bits_per_pixel == 16:
        pixel_value = struct.unpack_from('<H', row_data, pixel_offset)[0]
        red = _extract_masked_component(pixel_value, masks['red'])
        green = _extract_masked_component(pixel_value, masks['green'])
        blue = _extract_masked_component(pixel_value, masks['blue'])
        alpha = _extract_masked_component(pixel_value, masks['alpha'], 255)
        return blue, green, red, alpha

    raise ValueError(f"Unsupported bit depth: {bits_per_pixel}")


# Function to read a BMP file and convert it to BGRA bytes
def read_bmp_to_bytes(file_path):
    with open(file_path, 'rb') as bmp_file:
        # BMP file header (14 bytes)
        file_header = _read_exact(bmp_file, 14, 'BMP file header is incomplete')
        if file_header[0:2] != b'BM':
            raise ValueError('Invalid BMP file')

        data_offset = struct.unpack('<I', file_header[10:14])[0]

        # DIB header size (first 4 bytes of DIB header)
        dib_header_size_data = _read_exact(bmp_file, 4, 'BMP DIB header is incomplete')
        dib_header_size = struct.unpack('<I', dib_header_size_data)[0]
        if dib_header_size < 40:
            raise ValueError(f'Unsupported BMP DIB header size: {dib_header_size}')

        dib_rest = _read_exact(
            bmp_file,
            dib_header_size - 4,
            'BMP DIB header is incomplete'
        )
        dib_header = dib_header_size_data + dib_rest

        width = struct.unpack('<i', dib_header[4:8])[0]
        raw_height = struct.unpack('<i', dib_header[8:12])[0]
        planes = struct.unpack('<H', dib_header[12:14])[0]
        bits_per_pixel = struct.unpack('<H', dib_header[14:16])[0]
        compression = struct.unpack('<I', dib_header[16:20])[0]
        colors_used = struct.unpack('<I', dib_header[32:36])[0]

        if width <= 0 or raw_height == 0:
            raise ValueError(f'Invalid BMP dimensions: width={width}, height={raw_height}')

        if planes != 1:
            raise ValueError(f'Unsupported BMP planes count: {planes}')

        top_down = raw_height < 0
        height = abs(raw_height)

        if bits_per_pixel not in (16, 24, 32):
            raise ValueError(
                f'Unsupported bit depth: {bits_per_pixel}. Only 16-bit, 24-bit, or 32-bit BMP is supported'
            )

        if compression not in (0, 3):
            raise ValueError(
                f'Unsupported BMP compression: {compression}. Only BI_RGB (0) and BI_BITFIELDS (3) are supported'
            )

        # Parse color masks.
        # BI_RGB defaults:
        # - 24-bit: BGR
        # - 32-bit: BGRA / BGRX byte order on disk
        # - 16-bit: treat as RGB555 unless BI_BITFIELDS provides explicit masks
        masks = {
            'red': None,
            'green': None,
            'blue': None,
            'alpha': None,
        }

        if data_offset < bmp_file.tell():
            raise ValueError(
                f'Invalid BMP pixel data offset: {data_offset} is before the end of the header'
            )

        if compression == 3:
            if bits_per_pixel not in (16, 32):
                raise ValueError('BI_BITFIELDS is not supported for 24-bit BMP')
            masks = _read_bitfields_masks(
                bmp_file,
                dib_header,
                dib_header_size,
                data_offset,
                bits_per_pixel
            )
        else:
            if bits_per_pixel == 16:
                # Default BI_RGB for 16-bit BMP is commonly RGB555.
                masks['red'] = _normalize_mask(0x7C00)
                masks['green'] = _normalize_mask(0x03E0)
                masks['blue'] = _normalize_mask(0x001F)
                masks['alpha'] = None

        # Skip palette or any remaining header area before pixel data.
        bmp_file.seek(data_offset)

        bytes_per_pixel = bits_per_pixel // 8
        row_raw_size = width * bytes_per_pixel
        row_stride = ((row_raw_size + 3) // 4) * 4

        pixel_data = _read_exact(
            bmp_file,
            row_stride * height,
            'BMP pixel data is incomplete'
        )

        bgra_bytes = bytearray(width * height * 4)
        out_index = 0

        for y in range(height):
            src_y = y if top_down else (height - 1 - y)
            row_start = src_y * row_stride
            row_data = pixel_data[row_start:row_start + row_stride]

            for x in range(width):
                pixel_offset = x * bytes_per_pixel
                blue, green, red, alpha = _decode_rgb_pixel(
                    row_data,
                    pixel_offset,
                    bits_per_pixel,
                    compression,
                    masks
                )

                bgra_bytes[out_index] = blue
                bgra_bytes[out_index + 1] = green
                bgra_bytes[out_index + 2] = red
                bgra_bytes[out_index + 3] = alpha
                out_index += 4

        return bytes(bgra_bytes), (width, height)
