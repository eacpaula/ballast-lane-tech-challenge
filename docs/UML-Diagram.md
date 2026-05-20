flowchart LR
    %% Actors
    AnonymousVisitor["Actor: Anonymous Visitor"]
    GuestUser["Actor: Guest User"]
    Administrator["Actor: Administrator"]

    %% System Boundary
    subgraph BlogSystem["Blog Platform"]
        ReadPublicPosts(("Read Public Posts"))
        ReactToPosts(("Like / Dislike Posts"))

        CreatePost(("Create Posts"))
        EditOwnPost(("Edit Own Posts"))
        RemoveOwnPost(("Remove Own Posts"))
        CreateTags(("Create Tags"))

        ManageCategories(("Manage Post Categories"))
        ManageSettings(("Manage Settings"))
        ManageRoles(("Manage Roles"))
        ManagePermissions(("Manage Permissions"))
        ManageModules(("Manage Modules"))
        ManageUsers(("Manage Users"))

        GrantPermissions(("Grant Permissions"))
        ScopePermissions(("Scope Permissions to Modules"))
    end

    %% Anonymous Visitor Use Cases
    AnonymousVisitor --> ReadPublicPosts
    AnonymousVisitor --> ReactToPosts

    %% Guest User Use Cases
    GuestUser --> ReadPublicPosts
    GuestUser --> CreatePost
    GuestUser --> EditOwnPost
    GuestUser --> RemoveOwnPost
    GuestUser --> CreateTags
    GuestUser --> ReactToPosts

    %% Administrator Use Cases
    Administrator --> ManageCategories
    Administrator --> ManageSettings
    Administrator --> ManageRoles
    Administrator --> ManagePermissions
    Administrator --> ManageModules
    Administrator --> ManageUsers

    %% Business Capability Relationships
    ManageRoles -. includes .-> GrantPermissions
    ManagePermissions -. includes .-> ScopePermissions
    GrantPermissions -. uses .-> ManagePermissions
    ScopePermissions -. uses .-> ManageModules