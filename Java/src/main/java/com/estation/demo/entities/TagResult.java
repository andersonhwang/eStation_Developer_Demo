package com.estation.demo.entities;

import com.estation.demo.util.MessagePackUtil;
import java.util.List;

public record TagResult(
        String tagId,
        int rfPower,
        int battery,
        int version,
        int status,
        int token,
        int temperature,
        int channel,
        byte[] utcTime,
        byte[] timePercent,
        int count
) {
    public byte[] toMessagePack() {
        return MessagePackUtil.pack(List.of(
                tagId,
                rfPower,
                battery,
                version,
                status,
                token,
                temperature,
                channel,
                utcTime,
                timePercent,
                count
        ));
    }

    public static TagResult fromPacked(Object packed) {
        List<Object> values = packed instanceof byte[]
                ? MessagePackUtil.asList(MessagePackUtil.unpack((byte[]) packed))
                : MessagePackUtil.asList(packed);

        return new TagResult(
                MessagePackUtil.asString(values.get(0)),
                MessagePackUtil.asInt(values.get(1)),
                MessagePackUtil.asInt(values.get(2)),
                MessagePackUtil.asInt(values.get(3)),
                MessagePackUtil.asInt(values.get(4)),
                MessagePackUtil.asInt(values.get(5)),
                MessagePackUtil.asInt(values.get(6)),
                MessagePackUtil.asInt(values.get(7)),
                MessagePackUtil.asBinary(values.get(8)),
                MessagePackUtil.asBinary(values.get(9)),
                MessagePackUtil.asInt(values.get(10))
        );
    }
}
