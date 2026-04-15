package com.estation.demo.util;

import java.io.IOException;
import java.io.RandomAccessFile;
import java.nio.file.Path;
import java.util.Arrays;

public final class BmpReader {
    private BmpReader() {
    }

    public static ImageData readBmpToBgra(Path path) throws IOException {
        try (RandomAccessFile file = new RandomAccessFile(path.toFile(), "r")) {
            byte[] fileHeader = readExact(file, 14, "BMP file header is incomplete");
            if (fileHeader[0] != 'B' || fileHeader[1] != 'M') {
                throw new IOException("Invalid BMP file");
            }

            int dataOffset = littleEndianInt(fileHeader, 10);
            int dibHeaderSize = littleEndianInt(readExact(file, 4, "BMP DIB header is incomplete"), 0);
            if (dibHeaderSize < 40) {
                throw new IOException("Unsupported BMP DIB header size: " + dibHeaderSize);
            }

            byte[] dibRest = readExact(file, dibHeaderSize - 4, "BMP DIB header is incomplete");
            byte[] dibHeader = new byte[dibHeaderSize];
            System.arraycopy(intToLeBytes(dibHeaderSize), 0, dibHeader, 0, 4);
            System.arraycopy(dibRest, 0, dibHeader, 4, dibRest.length);

            int width = littleEndianInt(dibHeader, 4);
            int rawHeight = littleEndianInt(dibHeader, 8);
            int planes = littleEndianShort(dibHeader, 12);
            int bitsPerPixel = littleEndianShort(dibHeader, 14);
            int compression = littleEndianInt(dibHeader, 16);

            if (width <= 0 || rawHeight == 0) {
                throw new IOException("Invalid BMP dimensions: width=" + width + ", height=" + rawHeight);
            }
            if (planes != 1) {
                throw new IOException("Unsupported BMP planes count: " + planes);
            }
            if (bitsPerPixel != 16 && bitsPerPixel != 24 && bitsPerPixel != 32) {
                throw new IOException("Unsupported bit depth: " + bitsPerPixel);
            }
            if (compression != 0 && compression != 3) {
                throw new IOException("Unsupported BMP compression: " + compression);
            }

            boolean topDown = rawHeight < 0;
            int height = Math.abs(rawHeight);

            if (dataOffset < file.getFilePointer()) {
                throw new IOException("Invalid BMP pixel data offset: " + dataOffset);
            }

            MaskSet masks = new MaskSet(null, null, null, null);
            if (compression == 3) {
                if (bitsPerPixel != 16 && bitsPerPixel != 32) {
                    throw new IOException("BI_BITFIELDS is not supported for 24-bit BMP");
                }
                masks = readBitfieldsMasks(file, dibHeader, dibHeaderSize, dataOffset, bitsPerPixel);
            } else if (bitsPerPixel == 16) {
                masks = new MaskSet(
                        normalizeMask(0x7C00),
                        normalizeMask(0x03E0),
                        normalizeMask(0x001F),
                        null
                );
            }

            file.seek(dataOffset);

            int bytesPerPixel = bitsPerPixel / 8;
            int rowRawSize = width * bytesPerPixel;
            int rowStride = ((rowRawSize + 3) / 4) * 4;
            byte[] pixelData = readExact(file, rowStride * height, "BMP pixel data is incomplete");
            byte[] bgra = new byte[width * height * 4];
            int outIndex = 0;

            for (int y = 0; y < height; y++) {
                int srcY = topDown ? y : (height - 1 - y);
                int rowStart = srcY * rowStride;
                byte[] rowData = Arrays.copyOfRange(pixelData, rowStart, rowStart + rowStride);
                for (int x = 0; x < width; x++) {
                    int pixelOffset = x * bytesPerPixel;
                    Pixel pixel = decodePixel(rowData, pixelOffset, bitsPerPixel, compression, masks);
                    bgra[outIndex++] = (byte) pixel.blue;
                    bgra[outIndex++] = (byte) pixel.green;
                    bgra[outIndex++] = (byte) pixel.red;
                    bgra[outIndex++] = (byte) pixel.alpha;
                }
            }

            return new ImageData(bgra, width, height);
        }
    }

    private static MaskSet readBitfieldsMasks(RandomAccessFile file, byte[] dibHeader, int dibHeaderSize, int dataOffset, int bitsPerPixel)
            throws IOException {
        int redMask = 0;
        int greenMask = 0;
        int blueMask = 0;
        int alphaMask = 0;

        if (dibHeaderSize >= 52) {
            redMask = littleEndianInt(dibHeader, 40);
            greenMask = littleEndianInt(dibHeader, 44);
            blueMask = littleEndianInt(dibHeader, 48);
        }
        if (dibHeaderSize >= 56) {
            alphaMask = littleEndianInt(dibHeader, 52);
        }

        if (redMask != 0 || greenMask != 0 || blueMask != 0) {
            return new MaskSet(
                    normalizeMask(redMask),
                    normalizeMask(greenMask),
                    normalizeMask(blueMask),
                    normalizeMask(alphaMask)
            );
        }

        long currentPos = file.getFilePointer();
        long maskBytesAvailable = dataOffset - currentPos;
        if (maskBytesAvailable < 12) {
            throw new IOException("BMP bitfields masks are incomplete for " + bitsPerPixel + "-bit image");
        }

        byte[] maskData = readExact(file, 12, "BMP bitfields masks are incomplete");
        redMask = littleEndianInt(maskData, 0);
        greenMask = littleEndianInt(maskData, 4);
        blueMask = littleEndianInt(maskData, 8);

        long remaining = dataOffset - file.getFilePointer();
        if (remaining >= 4) {
            alphaMask = littleEndianInt(readExact(file, 4, "BMP alpha mask is incomplete"), 0);
        }

        return new MaskSet(
                normalizeMask(redMask),
                normalizeMask(greenMask),
                normalizeMask(blueMask),
                normalizeMask(alphaMask)
        );
    }

    private static Pixel decodePixel(byte[] rowData, int pixelOffset, int bitsPerPixel, int compression, MaskSet masks) {
        if (bitsPerPixel == 24) {
            return new Pixel(
                    Byte.toUnsignedInt(rowData[pixelOffset]),
                    Byte.toUnsignedInt(rowData[pixelOffset + 1]),
                    Byte.toUnsignedInt(rowData[pixelOffset + 2]),
                    255
            );
        }

        if (bitsPerPixel == 32) {
            int pixelValue = littleEndianInt(rowData, pixelOffset);
            if (compression == 0) {
                return new Pixel(
                        Byte.toUnsignedInt(rowData[pixelOffset]),
                        Byte.toUnsignedInt(rowData[pixelOffset + 1]),
                        Byte.toUnsignedInt(rowData[pixelOffset + 2]),
                        Byte.toUnsignedInt(rowData[pixelOffset + 3])
                );
            }
            return new Pixel(
                    extractMaskedComponent(pixelValue, masks.blue, 0),
                    extractMaskedComponent(pixelValue, masks.green, 0),
                    extractMaskedComponent(pixelValue, masks.red, 0),
                    extractMaskedComponent(pixelValue, masks.alpha, 255)
            );
        }

        int pixelValue = littleEndianShort(rowData, pixelOffset);
        return new Pixel(
                extractMaskedComponent(pixelValue, masks.blue, 0),
                extractMaskedComponent(pixelValue, masks.green, 0),
                extractMaskedComponent(pixelValue, masks.red, 0),
                extractMaskedComponent(pixelValue, masks.alpha, 255)
        );
    }

    private static int extractMaskedComponent(int pixelValue, MaskInfo maskInfo, int defaultValue) {
        if (maskInfo == null || maskInfo.maxValue == 0) {
            return defaultValue;
        }
        int value = (pixelValue >> maskInfo.shift) & maskInfo.maxValue;
        return (value * 255 + (maskInfo.maxValue / 2)) / maskInfo.maxValue;
    }

    private static MaskInfo normalizeMask(int mask) {
        if (mask == 0) {
            return null;
        }
        long unsignedMask = Integer.toUnsignedLong(mask);
        int shift = 0;
        long temp = unsignedMask;
        while ((temp & 1) == 0) {
            temp >>>= 1;
            shift++;
        }

        int bits = 0;
        while ((temp & 1) == 1) {
            temp >>>= 1;
            bits++;
        }

        int maxValue = bits > 0 ? (1 << bits) - 1 : 0;
        return new MaskInfo(shift, bits, maxValue);
    }

    private static byte[] readExact(RandomAccessFile file, int size, String message) throws IOException {
        byte[] data = new byte[size];
        int total = 0;
        while (total < size) {
            int read = file.read(data, total, size - total);
            if (read < 0) {
                throw new IOException(message);
            }
            total += read;
        }
        return data;
    }

    private static int littleEndianInt(byte[] bytes, int offset) {
        return (bytes[offset] & 0xFF)
                | ((bytes[offset + 1] & 0xFF) << 8)
                | ((bytes[offset + 2] & 0xFF) << 16)
                | ((bytes[offset + 3] & 0xFF) << 24);
    }

    private static int littleEndianShort(byte[] bytes, int offset) {
        return (bytes[offset] & 0xFF) | ((bytes[offset + 1] & 0xFF) << 8);
    }

    private static byte[] intToLeBytes(int value) {
        return new byte[] {
                (byte) (value & 0xFF),
                (byte) ((value >>> 8) & 0xFF),
                (byte) ((value >>> 16) & 0xFF),
                (byte) ((value >>> 24) & 0xFF)
        };
    }

    private record MaskInfo(int shift, int bits, int maxValue) {
    }

    private record MaskSet(MaskInfo red, MaskInfo green, MaskInfo blue, MaskInfo alpha) {
    }

    private record Pixel(int blue, int green, int red, int alpha) {
    }
}
