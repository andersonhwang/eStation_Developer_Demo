package com.estation.demo.entities;

import com.estation.demo.enums.PageIndex;
import com.estation.demo.enums.Pattern;
import com.estation.demo.util.MessagePackUtil;
import java.util.List;

public record EslEntity(
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
        String base64String
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
                base64String
        );
    }

    public byte[] toMessagePack() {
        return MessagePackUtil.pack(toPayloadList());
    }

    public static EslEntity fromMessagePack(byte[] data) {
        List<Object> values = MessagePackUtil.asList(MessagePackUtil.unpack(data));
        return new EslEntity(
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
                MessagePackUtil.asString(values.get(10))
        );
    }
}
