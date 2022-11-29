create table comments
(
    c_id           bigint identity,
    c_publish_date datetime2 not null,
    c_product_id   bigint,
    c_user_id      bigint,
    constraint PK_comments
        primary key (c_id),
    constraint FK_comments_products_c_product_id
        foreign key (c_product_id) references products,
    constraint FK_comments_users_c_user_id
        foreign key (c_user_id) references users
)
go

create index IX_comments_c_product_id
    on comments (c_product_id)
go

create index IX_comments_c_user_id
    on comments (c_user_id)
go

create table moderators
(
    u_id        bigint not null,
    m_full_name nvarchar(max),
    constraint PK_moderators
        primary key (u_id),
    constraint FK_moderators_users_u_id
        foreign key (u_id) references users
            on delete cascade
)
go

create table MSreplication_options
(
    optname          sysname not null,
    value            bit     not null,
    major_version    int     not null,
    minor_version    int     not null,
    revision         int     not null,
    install_failures int     not null
)
go

create table products
(
    pp_id             bigint identity,
    pp_publish_date   datetime2 not null,
    pp_price          float     not null,
    pp_download_count int       not null,
    pp_rating         float     not null,
    pp_publisher_id   bigint,
    pp_image_logo     varbinary,
    pp_image_preview  varbinary,
    constraint PK_products
        primary key (pp_id),
    constraint FK_products_publishers_pp_publisher_id
        foreign key (pp_publisher_id) references publishers
)
go

create index IX_products_pp_publisher_id
    on products (pp_publisher_id)
go

create table product_order
(
    po_id         bigint identity,
    po_product_id bigint,
    po_user_id    bigint,
    constraint PK_product_order
        primary key (po_id),
    constraint FK_product_order_products_po_product_id
        foreign key (po_product_id) references products,
    constraint FK_product_order_users_po_user_id
        foreign key (po_user_id) references users
)
go

create index IX_product_order_po_product_id
    on product_order (po_product_id)
go

create index IX_product_order_po_user_id
    on product_order (po_user_id)
go

create table publishers
(
    u_id        bigint        not null,
    p_full_name nvarchar(max) not null,
    constraint PK_publishers
        primary key (u_id),
    constraint FK_publishers_users_u_id
        foreign key (u_id) references users
            on delete cascade
)
go

create table spt_fallback_db
(
    xserver_name       varchar(30) not null,
    xdttm_ins          datetime    not null,
    xdttm_last_ins_upd datetime    not null,
    xfallback_dbid     smallint,
    name               varchar(30) not null,
    dbid               smallint    not null,
    status             smallint    not null,
    version            smallint    not null
)
go

grant select on spt_fallback_db to [public]
go

create table spt_fallback_dev
(
    xserver_name       varchar(30)  not null,
    xdttm_ins          datetime     not null,
    xdttm_last_ins_upd datetime     not null,
    xfallback_low      int,
    xfallback_drive    char(2),
    low                int          not null,
    high               int          not null,
    status             smallint     not null,
    name               varchar(30)  not null,
    phyname            varchar(127) not null
)
go

grant select on spt_fallback_dev to [public]
go

create table spt_fallback_usg
(
    xserver_name       varchar(30) not null,
    xdttm_ins          datetime    not null,
    xdttm_last_ins_upd datetime    not null,
    xfallback_vstart   int,
    dbid               smallint    not null,
    segmap             int         not null,
    lstart             int         not null,
    sizepg             int         not null,
    vstart             int         not null
)
go

grant select on spt_fallback_usg to [public]
go

create table spt_monitor
(
    lastrun       datetime not null,
    cpu_busy      int      not null,
    io_busy       int      not null,
    idle          int      not null,
    pack_received int      not null,
    pack_sent     int      not null,
    connections   int      not null,
    pack_errors   int      not null,
    total_read    int      not null,
    total_write   int      not null,
    total_errors  int      not null
)
go

grant select on spt_monitor to [public]
go


create view spt_values as
select name collate database_default as name,
	number,
	type collate database_default as type,
	low, high, status
from sys.spt_values
go

grant select on spt_values to [public]
go

create procedure dbo.sp_MScleanupmergepublisher
as
    exec sys.sp_MScleanupmergepublisher_internal
go


create procedure dbo.sp_MSrepl_startup
as
    exec sys.sp_MSrepl_startup_internal
go

create table users
(
    u_id       bigint identity,
    u_nickname nvarchar(max) not null,
    u_password nvarchar(max) not null,
    u_balance  float         not null,
    constraint PK_users
        primary key (u_id)
)
go
