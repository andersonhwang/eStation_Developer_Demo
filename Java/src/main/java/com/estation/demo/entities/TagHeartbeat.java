package com.estation.demo.entities;

import com.estation.demo.util.MessagePackUtil;
import java.util.List;

public record TagHeartbeat(
        String tagId,
        int rfPower,
        int battery,
        int version,
        int status,
        int token,
        int temperature,
        int channel,
        byte[] utcTime,
        int timePercent,
        int count,
        int factory,
        int color,
        int size,
        int type
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
                count,
                factory,
                color,
                size,
                type
        ));
    }

    public static TagHeartbeat fromPacked(Object packed) {
        List<Object> values = packed instanceof byte[]
                ? MessagePackUtil.asList(MessagePackUtil.unpack((byte[]) packed))
                : MessagePackUtil.asList(packed);

        return new TagHeartbeat(
                MessagePackUtil.asString(values.get(0)),
                MessagePackUtil.asInt(values.get(1)),
                MessagePackUtil.asInt(values.get(2)),
                MessagePackUtil.asInt(values.get(3)),
                MessagePackUtil.asInt(values.get(4)),
                MessagePackUtil.asInt(values.get(5)),
                MessagePackUtil.asInt(values.get(6)),
                MessagePackUtil.asInt(values.get(7)),
                MessagePackUtil.asBinary(values.get(8)),
                MessagePackUtil.asInt(values.get(9)),
                MessagePackUtil.asInt(values.get(10)),
                MessagePackUtil.asInt(values.get(11)),
                MessagePackUtil.asInt(values.get(12)),
                MessagePackUtil.asInt(values.get(13)),
                MessagePackUtil.asInt(values.get(14))
        );
    }
}
