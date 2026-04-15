package com.estation.demo.entities;

import com.estation.demo.util.MessagePackUtil;
import java.util.List;

public record TaskResult(
        int port,
        int waitCount,
        int sendCount,
        String message,
        List<TagResult> tags
) {
    public byte[] toMessagePack() {
        List<Object> tagPayloads = tags.stream().map(TagResult::toMessagePack).map(b -> (Object) b).toList();
        return MessagePackUtil.pack(List.of(
                port,
                waitCount,
                sendCount,
                message,
                tagPayloads
        ));
    }

    public static TaskResult fromMessagePack(byte[] data) {
        List<Object> values = MessagePackUtil.asList(MessagePackUtil.unpack(data));
        List<TagResult> tags = MessagePackUtil.asList(values.get(4)).stream()
                .map(TagResult::fromPacked)
                .toList();
        return new TaskResult(
                MessagePackUtil.asInt(values.get(0)),
                MessagePackUtil.asInt(values.get(1)),
                MessagePackUtil.asInt(values.get(2)),
                MessagePackUtil.asString(values.get(3)),
                tags
        );
    }
}
