package com.estation.demo.util;

import java.awt.image.BufferedImage;
import java.io.IOException;
import java.nio.file.Path;
import java.util.Locale;
import javax.imageio.ImageIO;

public final class ImageUtils {
    private ImageUtils() {
    }

    public static ImageData readImageBgra(Path path) throws IOException {
        String fileName = path.getFileName().toString().toLowerCase(Locale.ROOT);
        if (fileName.endsWith(".bmp")) {
            return BmpReader.readBmpToBgra(path);
        }

        BufferedImage image = ImageIO.read(path.toFile());
        if (image == null) {
            throw new IOException("Unsupported image format: " + path);
        }

        int width = image.getWidth();
        int height = image.getHeight();
        byte[] bgra = new byte[width * height * 4];
        int index = 0;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int argb = image.getRGB(x, y);
                bgra[index++] = (byte) (argb & 0xFF);
                bgra[index++] = (byte) ((argb >>> 8) & 0xFF);
                bgra[index++] = (byte) ((argb >>> 16) & 0xFF);
                bgra[index++] = (byte) ((argb >>> 24) & 0xFF);
            }
        }
        return new ImageData(bgra, width, height);
    }

    public static ImageData readImageBgr(Path path) throws IOException {
        String fileName = path.getFileName().toString().toLowerCase(Locale.ROOT);
        if (fileName.endsWith(".bmp")) {
            ImageData bgraData = BmpReader.readBmpToBgra(path);
            byte[] bgra = bgraData.bytes();
            int pixelCount = bgra.length / 4;
            byte[] bgr = new byte[pixelCount * 3];
            for (int i = 0; i < pixelCount; i++) {
                bgr[i * 3] = bgra[i * 4];
                bgr[i * 3 + 1] = bgra[i * 4 + 1];
                bgr[i * 3 + 2] = bgra[i * 4 + 2];
            }
            return new ImageData(bgr, bgraData.width(), bgraData.height());
        }

        BufferedImage image = ImageIO.read(path.toFile());
        if (image == null) {
            throw new IOException("Unsupported image format: " + path);
        }

        int width = image.getWidth();
        int height = image.getHeight();
        byte[] bgr = new byte[width * height * 3];
        int index = 0;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int argb = image.getRGB(x, y);
                bgr[index++] = (byte) (argb & 0xFF);
                bgr[index++] = (byte) ((argb >>> 8) & 0xFF);
                bgr[index++] = (byte) ((argb >>> 16) & 0xFF);
            }
        }
        return new ImageData(bgr, width, height);
    }
}
