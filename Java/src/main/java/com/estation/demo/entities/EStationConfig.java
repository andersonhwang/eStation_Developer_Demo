package com.estation.demo.entities;

import com.estation.demo.util.MessagePackUtil;
import java.util.ArrayList;
import java.util.List;

public record EStationConfig(
        String alias,
        String server,
        List<String> connParam,
        boolean encrypt,
        boolean autoIP,
        String localIP,
        String subnet,
        String gateway,
        int heartbeat
) {
    public byte[] toMessagePack() {
        return MessagePackUtil.pack(List.of(
                alias,
                server,
                connParam,
                encrypt,
                autoIP,
                localIP,
                subnet,
                gateway,
                heartbeat
        ));
    }

    public static EStationConfig fromMessagePack(byte[] data) {
        List<Object> values = MessagePackUtil.asList(MessagePackUtil.unpack(data));
        return new EStationConfig(
                MessagePackUtil.asString(values.get(0)),
                MessagePackUtil.asString(values.get(1)),
                MessagePackUtil.asStringList(values.get(2)),
                MessagePackUtil.asBoolean(values.get(3)),
                MessagePackUtil.asBoolean(values.get(4)),
                MessagePackUtil.asString(values.get(5)),
                MessagePackUtil.asString(values.get(6)),
                MessagePackUtil.asString(values.get(7)),
                MessagePackUtil.asInt(values.get(8))
        );
    }
}
