package com.estation.demo.entities;

import com.estation.demo.enums.Pattern;
import com.estation.demo.util.MessagePackUtil;
import java.util.List;

public record DslEntity2(
        String tagId,
        boolean r,
        boolean g,
        boolean b,
        int period,
        int interval,
        int duration,
        int token,
        byte[] hexData,
        Pattern pattern,
        String currentKey,
        String newKey
) {
    public byte[] toMessagePack() {
        return MessagePackUtil.pack(List.of(
                tagId,
                r,
                g,
                b,
                period,
                interval,
                duration,
                token,
                hexData,
                pattern.value(),
                currentKey,
                newKey
        ));
    }

    public static DslEntity2 fromMessagePack(byte[] data) {
        List<Object> values = MessagePackUtil.asList(MessagePackUtil.unpack(data));
        return new DslEntity2(
                MessagePackUtil.asString(values.get(0)),
                MessagePackUtil.asBoolean(values.get(1)),
                MessagePackUtil.asBoolean(values.get(2)),
                MessagePackUtil.asBoolean(values.get(3)),
                MessagePackUtil.asInt(values.get(4)),
                MessagePackUtil.asInt(values.get(5)),
                MessagePackUtil.asInt(values.get(6)),
                MessagePackUtil.asInt(values.get(7)),
                MessagePackUtil.asBinary(values.get(8)),
                Pattern.fromValue(MessagePackUtil.asInt(values.get(9))),
                MessagePackUtil.asString(values.get(10)),
                MessagePackUtil.asString(values.get(11))
        );
    }
}
