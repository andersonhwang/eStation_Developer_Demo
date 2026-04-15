package com.estation.demo.util;

import java.io.ByteArrayOutputStream;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;

public final class MessagePackUtil {
    private MessagePackUtil() {
    }

    public static byte[] pack(Object value) {
        ByteArrayOutputStream output = new ByteArrayOutputStream();
        writeValue(output, value);
        return output.toByteArray();
    }

    public static Object unpack(byte[] data) {
        return new Parser(data).readValue();
    }

    public static List<Object> asList(Object value) {
        if (value instanceof List<?> list) {
            return new ArrayList<>(list.stream().map(v -> (Object) v).toList());
        }
        throw new IllegalArgumentException("Expected list but got " + typeName(value));
    }

    public static List<String> asStringList(Object value) {
        return asList(value).stream().map(MessagePackUtil::asString).toList();
    }

    public static String asString(Object value) {
        if (value == null) {
            return "";
        }
        if (value instanceof String s) {
            return s;
        }
        if (value instanceof byte[] bytes) {
            return new String(bytes, StandardCharsets.UTF_8);
        }
        return String.valueOf(value);
    }

    public static boolean asBoolean(Object value) {
        if (value instanceof Boolean b) {
            return b;
        }
        if (value instanceof Number n) {
            return n.intValue() != 0;
        }
        throw new IllegalArgumentException("Expected boolean but got " + typeName(value));
    }

    public static int asInt(Object value) {
        if (value instanceof Number n) {
            return n.intValue();
        }
        throw new IllegalArgumentException("Expected integer but got " + typeName(value));
    }

    public static long asLong(Object value) {
        if (value instanceof Number n) {
            return n.longValue();
        }
        throw new IllegalArgumentException("Expected long but got " + typeName(value));
    }

    public static byte[] asBinary(Object value) {
        if (value instanceof byte[] bytes) {
            return bytes;
        }
        throw new IllegalArgumentException("Expected binary but got " + typeName(value));
    }

    private static void writeValue(ByteArrayOutputStream output, Object value) {
        if (value == null) {
            output.write(0xC0);
            return;
        }
        if (value instanceof Boolean b) {
            output.write(b ? 0xC3 : 0xC2);
            return;
        }
        if (value instanceof Byte || value instanceof Short || value instanceof Integer || value instanceof Long) {
            writeInteger(output, ((Number) value).longValue());
            return;
        }
        if (value instanceof String s) {
            writeString(output, s);
            return;
        }
        if (value instanceof byte[] bytes) {
            writeBinary(output, bytes);
            return;
        }
        if (value instanceof List<?> list) {
            writeArray(output, list);
            return;
        }
        throw new IllegalArgumentException("Unsupported MessagePack type: " + value.getClass().getName());
    }

    private static void writeInteger(ByteArrayOutputStream output, long value) {
        if (value >= 0 && value <= 0x7F) {
            output.write((int) value);
        } else if (value >= -32 && value < 0) {
            output.write((int) (value & 0xFF));
        } else if (value >= 0 && value <= 0xFF) {
            output.write(0xCC);
            output.write((int) value);
        } else if (value >= 0 && value <= 0xFFFF) {
            output.write(0xCD);
            writeUInt(output, value, 2);
        } else if (value >= 0 && value <= 0xFFFFFFFFL) {
            output.write(0xCE);
            writeUInt(output, value, 4);
        } else if (value >= Byte.MIN_VALUE && value <= Byte.MAX_VALUE) {
            output.write(0xD0);
            output.write((int) (value & 0xFF));
        } else if (value >= Short.MIN_VALUE && value <= Short.MAX_VALUE) {
            output.write(0xD1);
            writeSigned(output, value, 2);
        } else if (value >= Integer.MIN_VALUE && value <= Integer.MAX_VALUE) {
            output.write(0xD2);
            writeSigned(output, value, 4);
        } else {
            output.write(0xD3);
            writeSigned(output, value, 8);
        }
    }

    private static void writeString(ByteArrayOutputStream output, String value) {
        byte[] bytes = value.getBytes(StandardCharsets.UTF_8);
        int length = bytes.length;
        if (length <= 31) {
            output.write(0xA0 | length);
        } else if (length <= 0xFF) {
            output.write(0xD9);
            output.write(length);
        } else if (length <= 0xFFFF) {
            output.write(0xDA);
            writeUInt(output, length, 2);
        } else {
            output.write(0xDB);
            writeUInt(output, length, 4);
        }
        output.writeBytes(bytes);
    }

    private static void writeBinary(ByteArrayOutputStream output, byte[] value) {
        int length = value.length;
        if (length <= 0xFF) {
            output.write(0xC4);
            output.write(length);
        } else if (length <= 0xFFFF) {
            output.write(0xC5);
            writeUInt(output, length, 2);
        } else {
            output.write(0xC6);
            writeUInt(output, length, 4);
        }
        output.writeBytes(value);
    }

    private static void writeArray(ByteArrayOutputStream output, List<?> list) {
        int size = list.size();
        if (size <= 15) {
            output.write(0x90 | size);
        } else if (size <= 0xFFFF) {
            output.write(0xDC);
            writeUInt(output, size, 2);
        } else {
            output.write(0xDD);
            writeUInt(output, size, 4);
        }
        for (Object item : list) {
            writeValue(output, item);
        }
    }

    private static void writeUInt(ByteArrayOutputStream output, long value, int bytes) {
        for (int shift = (bytes - 1) * 8; shift >= 0; shift -= 8) {
            output.write((int) ((value >>> shift) & 0xFF));
        }
    }

    private static void writeSigned(ByteArrayOutputStream output, long value, int bytes) {
        for (int shift = (bytes - 1) * 8; shift >= 0; shift -= 8) {
            output.write((int) ((value >>> shift) & 0xFF));
        }
    }

    private static String typeName(Object value) {
        return value == null ? "null" : value.getClass().getName();
    }

    private static final class Parser {
        private final byte[] data;
        private int index;

        private Parser(byte[] data) {
            this.data = data;
        }

        private Object readValue() {
            int marker = readUnsignedByte();

            if ((marker & 0x80) == 0x00) {
                return marker;
            }
            if ((marker & 0xE0) == 0xE0) {
                return (byte) marker;
            }
            if ((marker & 0xF0) == 0x90) {
                return readArray(marker & 0x0F);
            }
            if ((marker & 0xE0) == 0xA0) {
                return readString(marker & 0x1F);
            }

            return switch (marker) {
                case 0xC0 -> null;
                case 0xC2 -> false;
                case 0xC3 -> true;
                case 0xC4 -> readBinary(readUnsignedByte());
                case 0xC5 -> readBinary((int) readUnsigned(2));
                case 0xC6 -> readBinary((int) readUnsigned(4));
                case 0xCC -> readUnsignedByte();
                case 0xCD -> (int) readUnsigned(2);
                case 0xCE -> readUnsigned(4);
                case 0xCF -> readUnsigned(8);
                case 0xD0 -> (int) (byte) readUnsignedByte();
                case 0xD1 -> (int) readSigned(2);
                case 0xD2 -> (int) readSigned(4);
                case 0xD3 -> readSigned(8);
                case 0xD9 -> readString(readUnsignedByte());
                case 0xDA -> readString((int) readUnsigned(2));
                case 0xDB -> readString((int) readUnsigned(4));
                case 0xDC -> readArray((int) readUnsigned(2));
                case 0xDD -> readArray((int) readUnsigned(4));
                default -> throw new IllegalArgumentException("Unsupported MessagePack marker: 0x" + Integer.toHexString(marker));
            };
        }

        private List<Object> readArray(int size) {
            List<Object> values = new ArrayList<>(size);
            for (int i = 0; i < size; i++) {
                values.add(readValue());
            }
            return values;
        }

        private String readString(int length) {
            String value = new String(data, index, length, StandardCharsets.UTF_8);
            index += length;
            return value;
        }

        private byte[] readBinary(int length) {
            byte[] value = new byte[length];
            System.arraycopy(data, index, value, 0, length);
            index += length;
            return value;
        }

        private long readUnsigned(int bytes) {
            long value = 0;
            for (int i = 0; i < bytes; i++) {
                value = (value << 8) | readUnsignedByte();
            }
            return value;
        }

        private long readSigned(int bytes) {
            long value = readUnsigned(bytes);
            long signBit = 1L << ((bytes * 8) - 1);
            if ((value & signBit) != 0) {
                value -= (1L << (bytes * 8));
            }
            return value;
        }

        private int readUnsignedByte() {
            return data[index++] & 0xFF;
        }
    }
}
