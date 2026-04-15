package com.estation.demo.entities;

import com.estation.demo.enums.MessageCode;
import com.estation.demo.util.MessagePackUtil;
import java.util.List;

public record EStationMessage(MessageCode code) {
    public byte[] toMessagePack() {
        return MessagePackUtil.pack(List.of(code.value()));
    }

    public static EStationMessage fromMessagePack(byte[] data) {
        List<Object> values = MessagePackUtil.asList(MessagePackUtil.unpack(data));
        return new EStationMessage(MessageCode.fromValue(MessagePackUtil.asInt(values.get(0))));
    }
}
