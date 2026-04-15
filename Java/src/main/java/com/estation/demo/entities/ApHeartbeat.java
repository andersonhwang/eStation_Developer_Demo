package com.estation.demo.entities;

import com.estation.demo.enums.MessageCode;
import com.estation.demo.util.MessagePackUtil;
import java.util.List;

public record ApHeartbeat(
        String id,
        int configVersion,
        String apVersion,
        String modVersion,
        MessageCode message,
        String messageEx,
        int waitCount,
        int sendCount,
        List<TagHeartbeat> tags
) {
    public byte[] toMessagePack() {
        List<Object> tagPayloads = tags.stream().map(TagHeartbeat::toMessagePack).map(b -> (Object) b).toList();
        return MessagePackUtil.pack(List.of(
                id,
                configVersion,
                apVersion,
                modVersion,
                message.value(),
                messageEx,
                waitCount,
                sendCount,
                tagPayloads
        ));
    }

    public static ApHeartbeat fromMessagePack(byte[] data) {
        List<Object> values = MessagePackUtil.asList(MessagePackUtil.unpack(data));
        List<TagHeartbeat> tags = MessagePackUtil.asList(values.get(8)).stream()
                .map(TagHeartbeat::fromPacked)
                .toList();
        return new ApHeartbeat(
                MessagePackUtil.asString(values.get(0)),
                MessagePackUtil.asInt(values.get(1)),
                MessagePackUtil.asString(values.get(2)),
                MessagePackUtil.asString(values.get(3)),
                MessageCode.fromValue(MessagePackUtil.asInt(values.get(4))),
                MessagePackUtil.asString(values.get(5)),
                MessagePackUtil.asInt(values.get(6)),
                MessagePackUtil.asInt(values.get(7)),
                tags
        );
    }
}
