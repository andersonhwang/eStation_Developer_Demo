package com.estation.demo.entities;

import com.estation.demo.enums.PageIndex;
import com.estation.demo.enums.Pattern;
import com.estation.demo.util.MessagePackUtil;
import java.util.List;

public record EslEntity2(
        String tagId,
        Pattern pattern,
        PageIndex pageIndex,
        boolean r,
        boolean g,
        boolean b,
        int times,
        int token,
        String currentKey,
        String newKey,
        byte[] bytes,
        boolean compress
) {
    public List<Object> toPayloadList() {
        return List.of(
                tagId,
                pattern.value(),
                pageIndex.value(),
                r,
                g,
                b,
                times,
                token,
                currentKey,
                newKey,
                bytes,
                compress
        );
    }

    public byte[] toMessagePack() {
        return MessagePackUtil.pack(toPayloadList());
    }

    public static EslEntity2 fromMessagePack(byte[] data) {
        List<Object> values = MessagePackUtil.asList(MessagePackUtil.unpack(data));
        return new EslEntity2(
                MessagePackUtil.asString(values.get(0)),
                Pattern.fromValue(MessagePackUtil.asInt(values.get(1))),
                PageIndex.fromValue(MessagePackUtil.asInt(values.get(2))),
                MessagePackUtil.asBoolean(values.get(3)),
                MessagePackUtil.asBoolean(values.get(4)),
                MessagePackUtil.asBoolean(values.get(5)),
                MessagePackUtil.asInt(values.get(6)),
                MessagePackUtil.asInt(values.get(7)),
                MessagePackUtil.asString(values.get(8)),
                MessagePackUtil.asString(values.get(9)),
                MessagePackUtil.asBinary(values.get(10)),
                MessagePackUtil.asBoolean(values.get(11))
        );
    }
}
