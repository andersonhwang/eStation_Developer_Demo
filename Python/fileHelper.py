import struct

# Function to read a BMP file and convert it to BGRA bytes
def read_bmp_to_bytes(file_path):
        
    with open(file_path, 'rb') as bmp_file:
        # BMP Header (14 Bytes)
        header = bmp_file.read(14)
        if header[0:2] != b'BM':
            raise ValueError("Invalid BMP file")

        # Offset
        data_offset = struct.unpack('<I', header[10:14])[0]

        # Header Information (40 Bytes)
        info_header = bmp_file.read(40)
        if len(info_header) < 40:
            raise ValueError("BMP Header Information is incomplete")

        # Image Information
        width = struct.unpack('<i', info_header[4:8])[0]
        height = struct.unpack('<i', info_header[8:12])[0]
        bits_per_pixel = struct.unpack('<H', info_header[14:16])[0]

        # Check if the bit depth is supported
        if bits_per_pixel not in [24, 32]:
            raise ValueError(f"Unsupported bit depth: {bits_per_pixel}. Only 24 or 32 bit BMP is supported")

        print(f"Image Size: {width} x {height}, Bit Depth: {bits_per_pixel}")

        # Jump to the pixel data start position
        bmp_file.seek(data_offset)

        # Row Size
        row_size = (width * bits_per_pixel + 31) // 32 * 4
        padding = row_size - (width * bits_per_pixel // 8)

        # Read pixels and convert to RGBA
        rgba_bytes = bytearray()
        
        for y in range(height):
            row_data = bmp_file.read(row_size)
            
            for x in range(width):
                pixel_pos = x * (bits_per_pixel // 8)

                if bits_per_pixel == 32:
                    # 32 Bit BMP - BGRA
                    blue = row_data[pixel_pos]
                    green = row_data[pixel_pos + 1]
                    red = row_data[pixel_pos + 2]
                    alpha = row_data[pixel_pos + 3]
                else:  
                    # 24 Bit BMP - BGR
                    blue = row_data[pixel_pos]
                    green = row_data[pixel_pos + 1]
                    red = row_data[pixel_pos + 2]
                    alpha = 255  
                
                rgba_bytes.extend([blue, green, red, alpha])
            bmp_file.read(padding)
        
        return bytes(rgba_bytes), (width, height)