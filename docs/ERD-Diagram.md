erDiagram
    USER ||--o{ POST : writes
    USER }o--o{ ROLE : assigned
    USER ||--o{ POST_REACTION : creates
    USER ||--o{ SETTING : manages

    ROLE }o--o{ PERMISSION : grants
    PERMISSION }o--|| MODULE : applies_to

    MODULE ||--o{ SETTING : contains

    POST_CATEGORY ||--o{ POST : categorizes
    POST }o--o{ TAG : labeled_with
    POST ||--o{ POST_REACTION : receives