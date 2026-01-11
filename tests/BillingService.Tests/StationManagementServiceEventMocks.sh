docker exec -it kafka bash

kafka-console-producer \
  --bootstrap-server kafka:9092 \
  --topic charging-session.events <<'EOF'
{"EventId":"3f6b0b1e-9d4c-4a6e-8a0a-6a9a2c1b4e01","EventType":"ChargingSessionStarted","EventVersion":1,"OccurredAt":"2026-01-09T10:00:00Z","Producer":"station-management-service","Key":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","Payload":{"SessionId":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","StationId":"a1c9f9b0-3f54-4f7d-9f1c-3a6c1b8f9a01","ProviderId":"b7e6c9d1-5e7a-4c9d-8a31-1c7f2d9b8a01","UserId":"c9d4e1a7-8b3e-4f2d-9c6a-2f8b1a7d4e01","StartedAt":"2026-01-09T10:00:00Z"}}
{"EventId":"5a9d6c3e-4f1b-4c9a-9e2d-7b8c1f2a3d02","EventType":"ChargingSessionUpdated","EventVersion":1,"OccurredAt":"2026-01-09T10:15:00Z","Producer":"station-management-service","Key":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","Payload":{"SessionId":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","ProviderId":"b7e6c9d1-5e7a-4c9d-8a31-1c7f2d9b8a01","EnergyConsumedKwh":4.75,"UpdatedAt":"2026-01-09T10:15:00Z"}}
{"EventId":"8c4a1d7e-2f6b-4a9c-8e3d-5b7f9c1a2e03","EventType":"ChargingSessionEnded","EventVersion":1,"OccurredAt":"2026-01-09T10:45:00Z","Producer":"station-management-service","Key":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","Payload":{"SessionId":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","ProviderId":"b7e6c9d1-5e7a-4c9d-8a31-1c7f2d9b8a01","TotalEnergyKwh":12.40,"EndedAt":"2026-01-09T10:45:00Z"}}

{"EventId":"2e9f6a7b-4c1d-4e3a-9b8f-6d1a7c5e4b04","EventType":"ChargingSessionStarted","EventVersion":1,"OccurredAt":"2026-01-09T11:05:00Z","Producer":"station-management-service","Key":"9f1c6a2d-8b7e-4f3a-9e5c-2a1b7d6c4e02","Payload":{"SessionId":"9f1c6a2d-8b7e-4f3a-9e5c-2a1b7d6c4e02","StationId":"d4c9a8b1-6f7e-4a5d-9c2b-3e1f7a6d8c02","ProviderId":"e8b1c7d6-5f4a-4e9b-8c2d-1a7f6b9e5c02","UserId":"f7a6c9b1-2e4d-4c8a-9b5f-1d7e6c3a2f02","StartedAt":"2026-01-09T11:05:00Z"}}
{"EventId":"7b4c6a1e-9f5d-4a8c-8e2b-3c1d7f6a5e05","EventType":"ChargingSessionUpdated","EventVersion":1,"OccurredAt":"2026-01-09T11:30:00Z","Producer":"station-management-service","Key":"9f1c6a2d-8b7e-4f3a-9e5c-2a1b7d6c4e02","Payload":{"SessionId":"9f1c6a2d-8b7e-4f3a-9e5c-2a1b7d6c4e02","ProviderId":"e8b1c7d6-5f4a-4e9b-8c2d-1a7f6b9e5c02","EnergyConsumedKwh":7.20,"UpdatedAt":"2026-01-09T11:30:00Z"}}
{"EventId":"1a7d9c4b-6f2e-4e8a-9c5d-3b7f1e6a2c06","EventType":"ChargingSessionEnded","EventVersion":1,"OccurredAt":"2026-01-09T12:00:00Z","Producer":"station-management-service","Key":"9f1c6a2d-8b7e-4f3a-9e5c-2a1b7d6c4e02","Payload":{"SessionId":"9f1c6a2d-8b7e-4f3a-9e5c-2a1b7d6c4e02","ProviderId":"e8b1c7d6-5f4a-4e9b-8c2d-1a7f6b9e5c02","TotalEnergyKwh":18.95,"EndedAt":"2026-01-09T12:00:00Z"}}
EOF


kafka-consumer-groups --bootstrap-server kafka:9092 --describe --group billing-service
kafka-console-consumer \
  --bootstrap-server kafka:9092 \
  --topic charging-session.events \
  --from-beginning


kafka-console-producer \
  --bootstrap-server localhost:9092 \
  --topic charging-session.events <<'EOF'
{"EventId":"8c4a1d7e-2f6b-4a9c-8e3d-5b7f9c1a2e03","EventType":"ChargingSessionEnded","EventVersion":1,"OccurredAt":"2026-01-09T10:45:00Z","Producer":"station-management-service","Key":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","Payload":{"SessionId":"6d7f3d44-7f42-4a3a-8e67-0c2a4c2f1a01","ProviderId":"b7e6c9d1-5e7a-4c9d-8a31-1c7f2d9b8a01","TotalEnergyKwh":12.40,"EndedAt":"2026-01-09T10:45:00Z"}}
EOF
