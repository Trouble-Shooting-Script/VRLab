```mermaid
graph TD
    A[레버 메커니즘으로 구현된 총기 케이스를 연다] --> B[총을 집으면 (GrabInteractor)];
    B --> C[게임 시작];
    C --> D[Interaction Event 트리거를 통해 총알 발사];
    D --> E{명중 여부 확인};
    E -- 명중 --> F[다음 과녁 생성];
    F --> D;
    E -- 빗나감 --> D;
```