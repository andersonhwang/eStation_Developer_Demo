package com.estation.demo.enums;

public enum PageIndex {
    P0(0),
    P1(1),
    P2(2),
    P3(3),
    P4(4),
    P5(5),
    P6(6),
    P7(7);

    private final int value;

    PageIndex(int value) {
        this.value = value;
    }

    public int value() {
        return value;
    }

    public static PageIndex fromValue(int value) {
        for (PageIndex pageIndex : values()) {
            if (pageIndex.value == value) {
                return pageIndex;
            }
        }
        throw new IllegalArgumentException("Unknown PageIndex value: " + value);
    }
}
