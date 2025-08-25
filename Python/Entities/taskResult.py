import msgpack
from dataclasses import dataclass, field
from typing import List
from Entities.tagResult import TagResult

@dataclass
class TaskResult:
    """
    Task result entity
    """
    Port: int = 0
    WaitCount: int = 0
    SendCount: int = 0
    Message: str = ""
    Tags: List[TagResult] = field(default_factory=list)

    def to_msgpack(self) -> bytes:
        data = [
            self.Port,
            self.WaitCount,
            self.SendCount,
            self.Message,
            [tag.to_msgpack() for tag in self.Tags]
        ]
        return msgpack.packb(data)

    @staticmethod
    def from_msgpack(packed: bytes):
        data = msgpack.unpackb(packed)
        tags = [TagResult.from_msgpack(tag_bytes) for tag_bytes in data[4]]
        return TaskResult(
            Port=data[0],
            WaitCount=data[1],
            SendCount=data[2],
            Message=data[3],
            Tags=tags
        )