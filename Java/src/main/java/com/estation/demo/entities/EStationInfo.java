package com.estation.demo.entities;

import com.estation.demo.util.MessagePackUtil;
import java.util.List;

public record EStationInfo(
        String id,
        String alias,
        String ip,
        String mac,
        int apType,
        String apVersion,
        String modVersion,
        int diskSize,
        int freeSpace,
        String server,
        List<String> connParam,
        boolean encrypt,
        boolean autoIP,
        String localIP,
        String subnet,
        String gateway,
        int heartbeat
) {
    public static EStationInfo fromMessagePack(byte[] data) {
        List<Object> values = MessagePackUtil.asList(MessagePackUtil.unpack(data));
        return new EStationInfo(
                MessagePackUtil.asString(values.get(0)),
                MessagePackUtil.asString(values.get(1)),
                MessagePackUtil.asString(values.get(2)),
                MessagePackUtil.asString(values.get(3)),
                MessagePackUtil.asInt(values.get(4)),
                MessagePackUtil.asString(values.get(5)),
                MessagePackUtil.asString(values.get(6)),
                MessagePackUtil.asInt(values.get(7)),
                MessagePackUtil.asInt(values.get(8)),
                MessagePackUtil.asString(values.get(9)),
                MessagePackUtil.asStringList(values.get(10)),
                MessagePackUtil.asBoolean(values.get(11)),
                MessagePackUtil.asBoolean(values.get(12)),
                MessagePackUtil.asString(values.get(13)),
                MessagePackUtil.asString(values.get(14)),
                MessagePackUtil.asString(values.get(15)),
                MessagePackUtil.asInt(values.get(16))
        );
    }
}
