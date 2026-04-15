package com.estation.demo.entities;

import com.estation.demo.util.MessagePackUtil;
import java.util.List;

public record OtaData(
        String downloadUrl,
        String confirmUrl,
        int type,
        String version,
        String name,
        String md5
) {
    public byte[] toMessagePack() {
        return MessagePackUtil.pack(List.of(
                downloadUrl,
                confirmUrl,
                type,
                version,
                name,
                md5
        ));
    }

    public static OtaData fromMessagePack(byte[] data) {
        List<Object> values = MessagePackUtil.asList(MessagePackUtil.unpack(data));
        return new OtaData(
                MessagePackUtil.asString(values.get(0)),
                MessagePackUtil.asString(values.get(1)),
                MessagePackUtil.asInt(values.get(2)),
                MessagePackUtil.asString(values.get(3)),
                MessagePackUtil.asString(values.get(4)),
                MessagePackUtil.asString(values.get(5))
        );
    }
}
