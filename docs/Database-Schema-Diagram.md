erDiagram
    USER {
        int id PK
        string fullname
        string bio
        date birthday
        boolean available
        string email UK
        string username UK
        string password
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    ROLE {
        int id PK
        string title UK
        string description
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    PERMISSION {
        int id PK
        string title
        string description
        int module_id FK
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    MODULE {
        int id PK
        string title UK
        string description
        boolean available
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    SETTING {
        int id PK
        string title UK
        string description
        boolean available
        string value
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    POST {
        int id PK
        int user_id FK
        int post_category_id FK
        string title
        string description
        text content
        int likes
        boolean available
        boolean public_post
        timestamp publish_date
        timestamp expire_date
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    POST_CATEGORY {
        int id PK
        string title UK
        string description
        boolean available
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    TAG {
        int id PK
        string title UK
        int created_by_user_id FK
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    USER_ROLE {
        int user_id PK, FK
        int role_id PK, FK
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    ROLE_PERMISSION {
        int role_id PK, FK
        int permission_id PK, FK
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    POST_TAG {
        int post_id PK, FK
        int tag_id PK, FK
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    POST_REACTION {
        int id PK
        int post_id FK
        int user_id FK
        string visitor_identifier
        string reaction_type
        timestamp reaction_date
        timestamp creation_date
        timestamp update_date
        int creation_user_id FK
        int update_user_id FK
    }

    USER ||--o{ POST : writes
    POST_CATEGORY ||--o{ POST : categorizes

    USER ||--o{ USER_ROLE : has
    ROLE ||--o{ USER_ROLE : assigned_to

    ROLE ||--o{ ROLE_PERMISSION : grants
    PERMISSION ||--o{ ROLE_PERMISSION : assigned_to_role

    MODULE ||--o{ PERMISSION : contains

    POST ||--o{ POST_TAG : has
    TAG ||--o{ POST_TAG : used_by

    POST ||--o{ POST_REACTION : receives
    USER ||--o{ POST_REACTION : creates

    USER ||--o{ TAG : creates

    USER ||--o{ USER : created_by
    USER ||--o{ USER : updated_by

    USER ||--o{ ROLE : created_by
    USER ||--o{ ROLE : updated_by

    USER ||--o{ PERMISSION : created_by
    USER ||--o{ PERMISSION : updated_by

    USER ||--o{ MODULE : created_by
    USER ||--o{ MODULE : updated_by

    USER ||--o{ SETTING : created_by
    USER ||--o{ SETTING : updated_by

    USER ||--o{ POST_CATEGORY : created_by
    USER ||--o{ POST_CATEGORY : updated_by