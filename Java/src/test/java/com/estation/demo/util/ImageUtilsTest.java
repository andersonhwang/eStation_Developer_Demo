package com.estation.demo.util;

import static org.junit.jupiter.api.Assertions.assertEquals;

import java.io.IOException;
import java.nio.file.Path;
import org.junit.jupiter.api.Test;

class ImageUtilsTest {
    private static final Path IMAGE_DIR = Path.of("src/main/java/com/estation/demo/images");

    @Test
    void readImageBgraSupportsBitfieldsBmp() throws IOException {
        ImageData image = ImageUtils.readImageBgra(IMAGE_DIR.resolve("T1.bmp"));

        assertEquals(296, image.width());
        assertEquals(152, image.height());
        assertEquals(296 * 152 * 4, image.bytes().length);
    }

    @Test
    void readImageBgraSupportsStandardBmp() throws IOException {
        ImageData image = ImageUtils.readImageBgra(IMAGE_DIR.resolve("T2.bmp"));

        assertEquals(800, image.width());
        assertEquals(480, image.height());
        assertEquals(800 * 480 * 4, image.bytes().length);
    }

    @Test
    void readImageBgraSupportsPngFallback() throws IOException {
        ImageData image = ImageUtils.readImageBgra(IMAGE_DIR.resolve("T1.png"));

        assertEquals(296, image.width());
        assertEquals(152, image.height());
        assertEquals(296 * 152 * 4, image.bytes().length);
    }
}
