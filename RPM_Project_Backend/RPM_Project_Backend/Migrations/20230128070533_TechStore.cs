using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPMProjectBackend.Migrations
{
    /// <inheritdoc />
    public partial class TechStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    adid = table.Column<int>(name: "ad_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    adstreet = table.Column<string>(name: "ad_street", type: "char(256)", unicode: false, fixedLength: true, maxLength: 256, nullable: false),
                    city = table.Column<string>(type: "char(256)", unicode: false, fixedLength: true, maxLength: 256, nullable: false),
                    state = table.Column<string>(type: "char(256)", unicode: false, fixedLength: true, maxLength: 256, nullable: false),
                    country = table.Column<string>(type: "char(256)", unicode: false, fixedLength: true, maxLength: 256, nullable: false),
                    aduid = table.Column<int>(name: "ad_u_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Addresses_pk", x => x.adid);
                });

            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    atrid = table.Column<int>(name: "atr_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    atrname = table.Column<string>(name: "atr_name", type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Attributes_pk", x => x.atrid);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    catid = table.Column<int>(name: "cat_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    catparentid = table.Column<int>(name: "cat_parent_id", type: "int", nullable: true),
                    catname = table.Column<string>(name: "cat_name", type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Categories_pk", x => x.catid);
                    table.ForeignKey(
                        name: "Categories_Categories_cat_id_fk",
                        column: x => x.catparentid,
                        principalTable: "Categories",
                        principalColumn: "cat_id");
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    pid = table.Column<int>(name: "p_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    presource = table.Column<string>(name: "p_resource", type: "char(128)", unicode: false, fixedLength: true, maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("permissions_pk", x => x.pid);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    rid = table.Column<int>(name: "r_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rname = table.Column<string>(name: "r_name", type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Roles_pk", x => x.rid);
                });

            migrationBuilder.CreateTable(
                name: "Categories_have_attributes",
                columns: table => new
                {
                    chaid = table.Column<int>(name: "cha_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chacatid = table.Column<int>(name: "cha_cat_id", type: "int", nullable: false),
                    chaatrid = table.Column<int>(name: "cha_atr_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Categories_have_attributes_pk", x => x.chaid);
                    table.ForeignKey(
                        name: "Categories_have_attributes_Attributes_atr_id_fk",
                        column: x => x.chaatrid,
                        principalTable: "Attributes",
                        principalColumn: "atr_id");
                    table.ForeignKey(
                        name: "Categories_have_attributes_Categories_cat_id_fk",
                        column: x => x.chacatid,
                        principalTable: "Categories",
                        principalColumn: "cat_id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    proid = table.Column<int>(name: "pro_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    proname = table.Column<string>(name: "pro_name", type: "char(128)", unicode: false, fixedLength: true, maxLength: 128, nullable: false),
                    proquantity = table.Column<int>(name: "pro_quantity", type: "int", nullable: false),
                    procost = table.Column<int>(name: "pro_cost", type: "int", nullable: false),
                    prodiscount = table.Column<int>(name: "pro_discount", type: "int", nullable: true),
                    procatid = table.Column<int>(name: "pro_cat_id", type: "int", nullable: false),
                    promanufacturer = table.Column<string>(name: "pro_manufacturer", type: "char(128)", unicode: false, fixedLength: true, maxLength: 128, nullable: false),
                    prophotospath = table.Column<string>(name: "pro_photos_path", type: "char(256)", unicode: false, fixedLength: true, maxLength: 256, nullable: false),
                    prorating = table.Column<double>(name: "pro_rating", type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Products_pk", x => x.proid);
                    table.ForeignKey(
                        name: "Products_Categories_cat_id_fk",
                        column: x => x.procatid,
                        principalTable: "Categories",
                        principalColumn: "cat_id");
                });

            migrationBuilder.CreateTable(
                name: "Role_has_permissions",
                columns: table => new
                {
                    rhpid = table.Column<int>(name: "rhp_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rhproleid = table.Column<int>(name: "rhp_role_id", type: "int", nullable: false),
                    rhppermissionid = table.Column<int>(name: "rhp_permission_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Role_has_permissions_pk", x => x.rhpid);
                    table.ForeignKey(
                        name: "Role_has_permissions_Permissions_p_id_fk",
                        column: x => x.rhppermissionid,
                        principalTable: "Permissions",
                        principalColumn: "p_id");
                    table.ForeignKey(
                        name: "Role_has_permissions_Roles_r_id_fk",
                        column: x => x.rhproleid,
                        principalTable: "Roles",
                        principalColumn: "r_id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    uid = table.Column<int>(name: "u_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ulogin = table.Column<string>(name: "u_login", type: "char(256)", unicode: false, fixedLength: true, maxLength: 256, nullable: false),
                    upassword = table.Column<string>(name: "u_password", type: "char(256)", unicode: false, fixedLength: true, maxLength: 256, nullable: false),
                    uemail = table.Column<string>(name: "u_email", type: "char(256)", unicode: false, fixedLength: true, maxLength: 256, nullable: false),
                    uroleid = table.Column<int>(name: "u_role_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Users_pk", x => x.uid);
                    table.ForeignKey(
                        name: "Users_Roles_r_id_fk",
                        column: x => x.uroleid,
                        principalTable: "Roles",
                        principalColumn: "r_id");
                });

            migrationBuilder.CreateTable(
                name: "Products_have_attributes",
                columns: table => new
                {
                    phaid = table.Column<int>(name: "pha_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    phaproid = table.Column<int>(name: "pha_pro_id", type: "int", nullable: false),
                    phaatrid = table.Column<int>(name: "pha_atr_id", type: "int", nullable: false),
                    phavalue = table.Column<string>(name: "pha_value", type: "varchar(256)", unicode: false, maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Products_have_attributes_pk", x => x.phaid);
                    table.ForeignKey(
                        name: "Products_have_attributes_Attributes_atr_id_fk",
                        column: x => x.phaatrid,
                        principalTable: "Attributes",
                        principalColumn: "atr_id");
                    table.ForeignKey(
                        name: "Products_have_attributes_Products_pro_id_fk",
                        column: x => x.phaproid,
                        principalTable: "Products",
                        principalColumn: "pro_id");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    payid = table.Column<int>(name: "pay_id", type: "int", nullable: false),
                    payuserid = table.Column<int>(name: "pay_user_id", type: "int", nullable: false),
                    paymethod = table.Column<string>(name: "pay_method", type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Payments_pk", x => x.payid);
                    table.ForeignKey(
                        name: "Payments_Users_u_id_fk",
                        column: x => x.payuserid,
                        principalTable: "Users",
                        principalColumn: "u_id");
                });

            migrationBuilder.CreateTable(
                name: "Product_lists",
                columns: table => new
                {
                    plid = table.Column<int>(name: "pl_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pluid = table.Column<int>(name: "pl_u_id", type: "int", nullable: false),
                    plname = table.Column<string>(name: "pl_name", type: "varchar(128)", unicode: false, maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Product_lists_pk", x => x.plid);
                    table.ForeignKey(
                        name: "Product_lists_Users_u_id_fk",
                        column: x => x.pluid,
                        principalTable: "Users",
                        principalColumn: "u_id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    rewid = table.Column<int>(name: "rew_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rewuid = table.Column<int>(name: "rew_u_id", type: "int", nullable: false),
                    rewtext = table.Column<string>(name: "rew_text", type: "varchar(max)", unicode: false, nullable: false),
                    rewproid = table.Column<int>(name: "rew_pro_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Reviews_pk", x => x.rewid);
                    table.ForeignKey(
                        name: "Reviews_Products_pro_id_fk",
                        column: x => x.rewproid,
                        principalTable: "Products",
                        principalColumn: "pro_id");
                    table.ForeignKey(
                        name: "Reviews_Users_u_id_fk",
                        column: x => x.rewuid,
                        principalTable: "Users",
                        principalColumn: "u_id");
                });

            migrationBuilder.CreateTable(
                name: "Sellers",
                columns: table => new
                {
                    suid = table.Column<int>(name: "su_id", type: "int", nullable: false),
                    sname = table.Column<string>(name: "s_name", type: "varchar(128)", unicode: false, maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Sellers_pk", x => x.suid);
                    table.ForeignKey(
                        name: "Sellers_Users_u_id_fk",
                        column: x => x.suid,
                        principalTable: "Users",
                        principalColumn: "u_id");
                });

            migrationBuilder.CreateTable(
                name: "Bank_cards",
                columns: table => new
                {
                    bcid = table.Column<int>(name: "bc_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bcnumber = table.Column<int>(name: "bc_number", type: "int", nullable: false),
                    bcname = table.Column<string>(name: "bc_name", type: "char(256)", unicode: false, fixedLength: true, maxLength: 256, nullable: false),
                    bcexpirationdate = table.Column<DateTime>(name: "bc_expiration_date", type: "date", nullable: false),
                    bccvc = table.Column<int>(name: "bc_cvc", type: "int", nullable: false),
                    bcpaymentid = table.Column<int>(name: "bc_payment_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Bank_cards_pk", x => x.bcid);
                    table.ForeignKey(
                        name: "Bank_cards_Payments_pay_id_fk",
                        column: x => x.bcpaymentid,
                        principalTable: "Payments",
                        principalColumn: "pay_id");
                });

            migrationBuilder.CreateTable(
                name: "Qiwi",
                columns: table => new
                {
                    qiwiid = table.Column<int>(name: "qiwi_id", type: "int", nullable: false),
                    qiwinumber = table.Column<int>(name: "qiwi_number", type: "int", nullable: true),
                    qiwipayid = table.Column<int>(name: "qiwi_pay_id", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Qiwi_pk", x => x.qiwiid);
                    table.ForeignKey(
                        name: "Qiwi_Payments_pay_id_fk",
                        column: x => x.qiwipayid,
                        principalTable: "Payments",
                        principalColumn: "pay_id");
                });

            migrationBuilder.CreateTable(
                name: "Lists_have_products",
                columns: table => new
                {
                    Lhpid = table.Column<int>(name: "Lhp_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lhpplid = table.Column<int>(name: "lhp_pl_id", type: "int", nullable: false),
                    lhpproid = table.Column<int>(name: "lhp_pro_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Lists_have_products_pk", x => x.Lhpid);
                    table.ForeignKey(
                        name: "Lists_have_products_Product_lists_pl_id_fk",
                        column: x => x.lhpplid,
                        principalTable: "Product_lists",
                        principalColumn: "pl_id");
                    table.ForeignKey(
                        name: "Lists_have_products_Products_pro_id_fk",
                        column: x => x.lhpproid,
                        principalTable: "Products",
                        principalColumn: "pro_id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    orid = table.Column<int>(name: "or_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ornumber = table.Column<int>(name: "or_number", type: "int", nullable: false),
                    orstatus = table.Column<string>(name: "or_status", type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    oradid = table.Column<int>(name: "or_ad_id", type: "int", nullable: false),
                    oruid = table.Column<int>(name: "or_u_id", type: "int", nullable: false),
                    orsid = table.Column<int>(name: "or_s_id", type: "int", nullable: false),
                    orfcost = table.Column<double>(name: "or_fcost", type: "float", nullable: false),
                    ortime = table.Column<DateTime>(name: "or_time", type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Orders_pk", x => x.orid);
                    table.ForeignKey(
                        name: "Orders_Addresses_ad_id_fk",
                        column: x => x.oradid,
                        principalTable: "Addresses",
                        principalColumn: "ad_id");
                    table.ForeignKey(
                        name: "Orders_Sellers_su_id_fk",
                        column: x => x.orsid,
                        principalTable: "Sellers",
                        principalColumn: "su_id");
                    table.ForeignKey(
                        name: "Orders_Users_u_id_fk",
                        column: x => x.oruid,
                        principalTable: "Users",
                        principalColumn: "u_id");
                });

            migrationBuilder.CreateTable(
                name: "Orders_have_products",
                columns: table => new
                {
                    ohpid = table.Column<int>(name: "ohp_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ohpproid = table.Column<int>(name: "ohp_pro_id", type: "int", nullable: false),
                    ohporid = table.Column<int>(name: "ohp_or_id", type: "int", nullable: true),
                    ohpquantity = table.Column<int>(name: "ohp_quantity", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Orders_have_products_pk", x => x.ohpid);
                    table.ForeignKey(
                        name: "Orders_have_products_Orders_or_id_fk",
                        column: x => x.ohporid,
                        principalTable: "Orders",
                        principalColumn: "or_id");
                    table.ForeignKey(
                        name: "Orders_have_products_Products_pro_id_fk",
                        column: x => x.ohpproid,
                        principalTable: "Products",
                        principalColumn: "pro_id");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    trid = table.Column<int>(name: "tr_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    trpayid = table.Column<int>(name: "tr_pay_id", type: "int", nullable: false),
                    trorid = table.Column<int>(name: "tr_or_id", type: "int", nullable: false),
                    trtime = table.Column<DateTime>(name: "tr_time", type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("transactions_pk", x => x.trid);
                    table.ForeignKey(
                        name: "Transactions_Orders_or_id_fk",
                        column: x => x.trorid,
                        principalTable: "Orders",
                        principalColumn: "or_id");
                    table.ForeignKey(
                        name: "transactions_Payments_pay_id_fk",
                        column: x => x.trpayid,
                        principalTable: "Payments",
                        principalColumn: "pay_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bank_cards_bc_payment_id",
                table: "Bank_cards",
                column: "bc_payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_cat_parent_id",
                table: "Categories",
                column: "cat_parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_have_attributes_cha_atr_id",
                table: "Categories_have_attributes",
                column: "cha_atr_id");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_have_attributes_cha_cat_id",
                table: "Categories_have_attributes",
                column: "cha_cat_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lists_have_products_lhp_pl_id",
                table: "Lists_have_products",
                column: "lhp_pl_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lists_have_products_lhp_pro_id",
                table: "Lists_have_products",
                column: "lhp_pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_or_ad_id",
                table: "Orders",
                column: "or_ad_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_or_s_id",
                table: "Orders",
                column: "or_s_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_or_u_id",
                table: "Orders",
                column: "or_u_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_have_products_ohp_or_id",
                table: "Orders_have_products",
                column: "ohp_or_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_have_products_ohp_pro_id",
                table: "Orders_have_products",
                column: "ohp_pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_pay_user_id",
                table: "Payments",
                column: "pay_user_id");

            migrationBuilder.CreateIndex(
                name: "permissions_resource_uq",
                table: "Permissions",
                column: "p_resource",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_lists_pl_u_id",
                table: "Product_lists",
                column: "pl_u_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_pro_cat_id",
                table: "Products",
                column: "pro_cat_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_have_attributes_pha_atr_id",
                table: "Products_have_attributes",
                column: "pha_atr_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_have_attributes_pha_pro_id",
                table: "Products_have_attributes",
                column: "pha_pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Qiwi_qiwi_pay_id",
                table: "Qiwi",
                column: "qiwi_pay_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_rew_pro_id",
                table: "Reviews",
                column: "rew_pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_rew_u_id",
                table: "Reviews",
                column: "rew_u_id");

            migrationBuilder.CreateIndex(
                name: "IX_Role_has_permissions_rhp_permission_id",
                table: "Role_has_permissions",
                column: "rhp_permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_Role_has_permissions_rhp_role_id",
                table: "Role_has_permissions",
                column: "rhp_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_tr_or_id",
                table: "Transactions",
                column: "tr_or_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_tr_pay_id",
                table: "Transactions",
                column: "tr_pay_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_u_role_id",
                table: "Users",
                column: "u_role_id");

            migrationBuilder.CreateIndex(
                name: "Users_email_unique",
                table: "Users",
                column: "u_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Users_login_unique",
                table: "Users",
                column: "u_login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bank_cards");

            migrationBuilder.DropTable(
                name: "Categories_have_attributes");

            migrationBuilder.DropTable(
                name: "Lists_have_products");

            migrationBuilder.DropTable(
                name: "Orders_have_products");

            migrationBuilder.DropTable(
                name: "Products_have_attributes");

            migrationBuilder.DropTable(
                name: "Qiwi");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Role_has_permissions");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Product_lists");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Sellers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
