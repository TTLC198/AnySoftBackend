create table Addresses
(
    ad_id     int identity
        constraint Addresses_pk
            primary key,
    ad_street char(256) not null,
    city      char(256) not null,
    state     char(256) not null,
    country   char(256) not null,
    ad_u_id   int       not null
)
go

create table Attributes
(
    atr_id   int identity
        constraint Attributes_pk
            primary key,
    atr_name char(50) not null
)
go

create table Categories
(
    cat_id        int identity
        constraint Categories_pk
            primary key,
    cat_parent_id int
        constraint Categories_Categories_cat_id_fk
            references Categories,
    cat_name      char(50) not null
)
go

create table Categories_have_attributes
(
    cha_id     int identity
        constraint Categories_have_attributes_pk
            primary key,
    cha_cat_id int not null
        constraint Categories_have_attributes_Categories_cat_id_fk
            references Categories,
    cha_atr_id int not null
        constraint Categories_have_attributes_Attributes_atr_id_fk
            references Attributes
)
go

create table Permissions
(
    p_id       int identity
        constraint permissions_pk
            primary key,
    p_resource char(128) not null
        constraint permissions_resource_uq
            unique
)
go

create table Products
(
    pro_id           int identity
        constraint Products_pk
            primary key,
    pro_name         char(128) not null,
    pro_quantity     int       not null,
    pro_cost         int       not null,
    pro_discount     int,
    pro_cat_id       int       not null
        constraint Products_Categories_cat_id_fk
            references Categories,
    pro_manufacturer char(128) not null,
    pro_photos_path  char(256) not null,
    pro_rating       float     not null
)
go

create table Products_have_attributes
(
    pha_id     int identity
        constraint Products_have_attributes_pk
            primary key,
    pha_pro_id int          not null
        constraint Products_have_attributes_Products_pro_id_fk
            references Products,
    pha_atr_id int          not null
        constraint Products_have_attributes_Attributes_atr_id_fk
            references Attributes,
    pha_value  varchar(256) not null
)
go

create table Roles
(
    r_id   int identity
        constraint Roles_pk
            primary key,
    r_name char not null
)
go

create table Role_has_permissions
(
    rhp_role_id       int not null
        constraint Role_has_permissions_Roles_r_id_fk
            references Roles,
    rhp_permission_id int not null
        constraint Role_has_permissions_Permissions_p_id_fk
            references Permissions,
    rhp_id            int identity
        constraint Role_has_permissions_pk
            primary key
)
go

create table Users
(
    u_login    char(256) not null
        constraint Users_login_unique
            unique,
    u_password char(256) not null,
    u_email    char(256) not null
        constraint Users_email_unique
            unique,
    u_role_id  int       not null
        constraint Users_Roles_r_id_fk
            references Roles,
    u_id       int identity
        constraint Users_pk
            primary key
)
go

create table Payments
(
    pay_id      int      not null
        constraint Payments_pk
            primary key,
    pay_user_id int      not null
        constraint Payments_Users_u_id_fk
            references Users,
    pay_method  char(50) not null
)
go

create table Bank_cards
(
    bc_number          int       not null,
    bc_name            char(256) not null,
    bc_expiration_date date      not null,
    bc_cvc             int       not null,
    bc_payment_id      int       not null
        constraint Bank_cards_Payments_pay_id_fk
            references Payments,
    bc_id              int identity
        constraint Bank_cards_pk
            primary key
)
go

create table Product_lists
(
    pl_id   int identity
        constraint Product_lists_pk
            primary key,
    pl_u_id int          not null
        constraint Product_lists_Users_u_id_fk
            references Users,
    pl_name varchar(128) not null
)
go

create table Lists_have_products
(
    Lhp_id     int identity
        constraint Lists_have_products_pk
            primary key,
    lhp_pl_id  int not null
        constraint Lists_have_products_Product_lists_pl_id_fk
            references Product_lists,
    lhp_pro_id int not null
        constraint Lists_have_products_Products_pro_id_fk
            references Products
)
go

create table Qiwi
(
    qiwi_id     int not null
        constraint Qiwi_pk
            primary key,
    qiwi_number int,
    qiwi_pay_id int
        constraint Qiwi_Payments_pay_id_fk
            references Payments
)
go

create table Reviews
(
    rew_id     int identity
        constraint Reviews_pk
            primary key,
    rew_u_id   int          not null
        constraint Reviews_Users_u_id_fk
            references Users,
    rew_text   varchar(max) not null,
    rew_pro_id int          not null
        constraint Reviews_Products_pro_id_fk
            references Products
)
go

create table Sellers
(
    su_id  int          not null
        constraint Sellers_pk
            primary key
        constraint Sellers_Users_u_id_fk
            references Users,
    s_name varchar(128) not null
)
go

create table Orders
(
    or_id     int identity
        constraint Orders_pk
            primary key,
    or_number int          not null,
    or_status varchar(128) not null,
    or_ad_id  int          not null
        constraint Orders_Addresses_ad_id_fk
            references Addresses,
    or_u_id   int          not null
        constraint Orders_Users_u_id_fk
            references Users,
    or_s_id   int          not null
        constraint Orders_Sellers_su_id_fk
            references Sellers,
    or_fcost  float        not null,
    or_time   datetime     not null
)
go

create table Orders_have_products
(
    ohp_id       int identity
        constraint Orders_have_products_pk
            primary key,
    ohp_pro_id   int not null
        constraint Orders_have_products_Products_pro_id_fk
            references Products,
    ohp_or_id    int
        constraint Orders_have_products_Orders_or_id_fk
            references Orders,
    ohp_quantity int not null
)
go

create table Transactions
(
    tr_id     int identity
        constraint transactions_pk
            primary key,
    tr_pay_id int      not null
        constraint transactions_Payments_pay_id_fk
            references Payments,
    tr_or_id  int      not null
        constraint Transactions_Orders_or_id_fk
            references Orders,
    tr_time   datetime not null
)
go


