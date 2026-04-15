package com.estation.demo.entities;

import com.estation.demo.enums.ColorType;

public record TagType(
        String code,
        int width,
        int height,
        String type,
        ColorType color
) {
    public int length() {
        return height * width * 4;
    }

    public int length2() {
        int widthAligned = (width % 8) == 0 ? width : (((width / 8) + 1) * 8);
        return height * widthAligned / 8;
    }
}
