using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTLtruc_quan.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "amenities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    icon = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_amenities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "court_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    icon = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_court_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "email_queue",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    to_email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    template = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    retry_count = table.Column<int>(type: "int", nullable: false),
                    send_after = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    sent_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    error = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_queue", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job_runs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    job_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    started_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    finished_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    affected_rows = table.Column<int>(type: "int", nullable: false),
                    error = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_runs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    actor_user_id = table.Column<long>(type: "bigint", nullable: true),
                    actor_role = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    performed_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    action = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    entity_type = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    entity_id = table.Column<long>(type: "bigint", nullable: false),
                    old_value = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    new_value = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    ip = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    user_agent = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auth_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    token_hash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    used_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    revoked_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auth_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "booking_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<long>(type: "bigint", nullable: false),
                    action = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    old_value = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    new_value = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    price_diff = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    performed_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    performed_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "booking_reminders",
                columns: table => new
                {
                    booking_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_reminders", x => new { x.booking_id, x.type });
                });

            migrationBuilder.CreateTable(
                name: "booking_services",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<long>(type: "bigint", nullable: false),
                    service_id = table.Column<long>(type: "bigint", nullable: false),
                    name_snapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price_snapshot = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    added_when = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    added_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    added_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_services", x => x.id);
                    table.CheckConstraint("CK_booking_services_qty", "[quantity] > 0 AND [unit_price_snapshot] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "booking_slots",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<long>(type: "bigint", nullable: false),
                    court_id = table.Column<long>(type: "bigint", nullable: false),
                    slot_id = table.Column<long>(type: "bigint", nullable: false),
                    slot_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    end_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    slot_type_snapshot = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    day_type_snapshot = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    price_snapshot = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_slots", x => x.id);
                    table.CheckConstraint("CK_booking_slots_price", "[price_snapshot] >= 0");
                    table.CheckConstraint("CK_booking_slots_time", "[end_at] > [start_at]");
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    source = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    payment_status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    play_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    end_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    slot_amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    service_amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    discount_amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    deposit_percent_snapshot = table.Column<int>(type: "int", nullable: false),
                    deposit_amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    paid_amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    refund_amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    cancel_policy_snapshot = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    hold_expires_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    recurring_id = table.Column<long>(type: "bigint", nullable: true),
                    voucher_id = table.Column<long>(type: "bigint", nullable: true),
                    qr_secret = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    checked_in_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    checkin_method = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    checked_in_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    completed_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    cancel_type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    cancel_reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    cancelled_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    cancelled_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: false),
                    created_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookings", x => x.id);
                    table.CheckConstraint("CK_bookings_cancel", "[status] NOT IN ('Cancelled','Expired','NoShow') OR ([cancel_type] IS NOT NULL AND [cancelled_at] IS NOT NULL)");
                    table.CheckConstraint("CK_bookings_cancel_reason", "[cancel_type] NOT IN ('Manager','Admin') OR LEN(LTRIM(RTRIM([cancel_reason]))) > 0");
                    table.CheckConstraint("CK_bookings_checkin", "[status] NOT IN ('CheckedIn','Completed') OR [checked_in_at] IS NOT NULL");
                    table.CheckConstraint("CK_bookings_deposit", "[deposit_amount] <= [total_amount] AND [refund_amount] <= [paid_amount]");
                    table.CheckConstraint("CK_bookings_hold", "[status] <> 'Holding' OR [hold_expires_at] IS NOT NULL");
                    table.CheckConstraint("CK_bookings_money", "[slot_amount] >= 0 AND [service_amount] >= 0 AND [discount_amount] >= 0 AND [deposit_amount] >= 0 AND [paid_amount] >= 0 AND [refund_amount] >= 0");
                    table.CheckConstraint("CK_bookings_percent", "[deposit_percent_snapshot] BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_bookings_status", "[status] IN ('Holding','Deposited','CheckedIn','Completed','Cancelled','Expired','NoShow')");
                    table.CheckConstraint("CK_bookings_time", "[end_at] > [start_at]");
                    table.CheckConstraint("CK_bookings_total", "[total_amount] = [slot_amount] + [service_amount] - [discount_amount]");
                });

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<long>(type: "bigint", nullable: false),
                    invoice_no = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    issued_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    issued_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    pdf_path = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoices", x => x.id);
                    table.CheckConstraint("CK_invoices_total", "[total_amount] >= 0");
                    table.ForeignKey(
                        name: "fk_invoices_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    method = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    gateway = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    gateway_txn_id = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    gateway_payload = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    related_payment_id = table.Column<long>(type: "bigint", nullable: true),
                    received_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    paid_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.id);
                    table.CheckConstraint("CK_payments_amount", "[amount] >= 0");
                    table.ForeignKey(
                        name: "fk_payments_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payments_payments_related_payment_id",
                        column: x => x.related_payment_id,
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "invoice_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    invoice_id = table.Column<long>(type: "bigint", nullable: false),
                    item_type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_items_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "branch_amenities",
                columns: table => new
                {
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    amenity_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch_amenities", x => new { x.branch_id, x.amenity_id });
                    table.ForeignKey(
                        name: "fk_branch_amenities_amenities_amenity_id",
                        column: x => x.amenity_id,
                        principalTable: "amenities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "branch_images",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    is_cover = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch_images", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "branch_policies",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    deposit_percent = table.Column<int>(type: "int", nullable: false),
                    allow_change_booking = table.Column<bool>(type: "bit", nullable: false),
                    change_before_hours = table.Column<int>(type: "int", nullable: false),
                    max_hold_extend = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch_policies", x => x.id);
                    table.CheckConstraint("CK_branch_policies_deposit", "[deposit_percent] BETWEEN 0 AND 100");
                });

            migrationBuilder.CreateTable(
                name: "branch_staff_names",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch_staff_names", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "branches",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    owner_user_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    district = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    latitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    longitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    open_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    close_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    reject_reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    lock_reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    approved_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    avg_rating = table.Column<decimal>(type: "decimal(2,1)", precision: 2, scale: 1, nullable: false, defaultValue: 0.0m),
                    deleted_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branches", x => x.id);
                    table.CheckConstraint("CK_branches_lat_long", "([latitude] IS NULL OR [latitude] BETWEEN -90 AND 90) AND ([longitude] IS NULL OR [longitude] BETWEEN -180 AND 180)");
                    table.CheckConstraint("CK_branches_open_close", "[open_time] < [close_time]");
                });

            migrationBuilder.CreateTable(
                name: "cancel_policy_tiers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    hours_before = table.Column<int>(type: "int", nullable: false),
                    refund_percent = table.Column<int>(type: "int", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cancel_policy_tiers", x => x.id);
                    table.CheckConstraint("CK_cancel_policy_tiers_refund", "[refund_percent] BETWEEN 0 AND 100");
                    table.ForeignKey(
                        name: "fk_cancel_policy_tiers_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "courts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    court_type_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    image_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    live_status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    live_status_manual_until = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_courts", x => x.id);
                    table.CheckConstraint("CK_courts_capacity", "[capacity] > 0");
                    table.ForeignKey(
                        name: "fk_courts_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_courts_court_types_court_type_id",
                        column: x => x.court_type_id,
                        principalTable: "court_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "daily_closings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    closing_date = table.Column<DateOnly>(type: "date", nullable: false),
                    cash_total = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    transfer_total = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    card_total = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    actual_cash = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    difference = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    closed_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_closings", x => x.id);
                    table.ForeignKey(
                        name: "fk_daily_closings_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "holidays",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    holiday_date = table.Column<DateOnly>(type: "date", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    surcharge_percent = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_holidays", x => x.id);
                    table.CheckConstraint("CK_holidays_surcharge", "[surcharge_percent] >= 0");
                    table.ForeignKey(
                        name: "fk_holidays_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    price = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    stock_qty = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_services", x => x.id);
                    table.CheckConstraint("CK_services_price", "[price] >= 0");
                    table.ForeignKey(
                        name: "fk_services_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "time_slots",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    slot_index = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    slot_type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_time_slots", x => x.id);
                    table.ForeignKey(
                        name: "fk_time_slots_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    password_hash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    avatar_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    is_walk_in = table.Column<bool>(type: "bit", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    lock_reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    email_verified_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    no_show_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    booking_restricted_until = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    require_full_prepay = table.Column<bool>(type: "bit", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.CheckConstraint("CK_users_branch_role", "([role_id] = 2 AND [branch_id] IS NOT NULL) OR ([role_id] <> 2 AND [branch_id] IS NULL)");
                    table.CheckConstraint("CK_users_no_show", "[no_show_count] >= 0");
                    table.CheckConstraint("CK_users_status", "[status] IN ('Active','Locked')");
                    table.ForeignKey(
                        name: "fk_users_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vouchers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    discount_type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    value = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    max_discount = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: true),
                    min_order = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: true),
                    usage_limit = table.Column<int>(type: "int", nullable: true),
                    used_count = table.Column<int>(type: "int", nullable: false),
                    valid_from = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    valid_to = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vouchers", x => x.id);
                    table.CheckConstraint("CK_vouchers_value", "[value] >= 0");
                    table.ForeignKey(
                        name: "fk_vouchers_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    court_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment", x => x.id);
                    table.ForeignKey(
                        name: "fk_equipment_courts_court_id",
                        column: x => x.court_id,
                        principalTable: "courts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "price_rules",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    court_type_id = table.Column<long>(type: "bigint", nullable: true),
                    court_id = table.Column<long>(type: "bigint", nullable: true),
                    day_type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    slot_type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    price = table.Column<decimal>(type: "decimal(12,0)", precision: 12, scale: 0, nullable: false),
                    valid_from = table.Column<DateOnly>(type: "date", nullable: false),
                    valid_to = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_price_rules", x => x.id);
                    table.CheckConstraint("CK_price_rules_dates", "[valid_to] IS NULL OR [valid_to] >= [valid_from]");
                    table.CheckConstraint("CK_price_rules_price", "[price] >= 0");
                    table.CheckConstraint("CK_price_rules_target", "[court_id] IS NOT NULL OR [court_type_id] IS NOT NULL");
                    table.ForeignKey(
                        name: "fk_price_rules_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_price_rules_court_types_court_type_id",
                        column: x => x.court_type_id,
                        principalTable: "court_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_price_rules_courts_court_id",
                        column: x => x.court_id,
                        principalTable: "courts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "slot_blocks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    court_id = table.Column<long>(type: "bigint", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_slot_id = table.Column<long>(type: "bigint", nullable: true),
                    end_slot_id = table.Column<long>(type: "bigint", nullable: true),
                    block_type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    performed_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_slot_blocks", x => x.id);
                    table.CheckConstraint("CK_slot_blocks_dates", "[end_date] >= [start_date]");
                    table.CheckConstraint("CK_slot_blocks_slots", "([start_slot_id] IS NULL AND [end_slot_id] IS NULL) OR ([start_slot_id] IS NOT NULL AND [end_slot_id] IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_slot_blocks_courts_court_id",
                        column: x => x.court_id,
                        principalTable: "courts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_slot_blocks_time_slots_end_slot_id",
                        column: x => x.end_slot_id,
                        principalTable: "time_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_slot_blocks_time_slots_start_slot_id",
                        column: x => x.start_slot_id,
                        principalTable: "time_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "broadcasts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sent_by_user_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: false),
                    target = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_broadcasts", x => x.id);
                    table.ForeignKey(
                        name: "fk_broadcasts_users_sent_by_user_id",
                        column: x => x.sent_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chat_threads",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: true),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    last_message_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_threads", x => x.id);
                    table.ForeignKey(
                        name: "fk_chat_threads_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_chat_threads_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "complaints",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    booking_id = table.Column<long>(type: "bigint", nullable: true),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    escalated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    resolution = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    resolved_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_complaints", x => x.id);
                    table.ForeignKey(
                        name: "fk_complaints_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_complaints_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_complaints_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_complaints_users_resolved_by_user_id",
                        column: x => x.resolved_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customer_violations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    booking_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    action_taken = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    resolved_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_violations", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_violations_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_customer_violations_users_resolved_by_user_id",
                        column: x => x.resolved_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_customer_violations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "favorites",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    court_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_favorites", x => x.id);
                    table.ForeignKey(
                        name: "fk_favorites_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_favorites_courts_court_id",
                        column: x => x.court_id,
                        principalTable: "courts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_favorites_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    body = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: false),
                    link = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recurring_bookings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    court_id = table.Column<long>(type: "bigint", nullable: false),
                    weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    slot_id = table.Column<long>(type: "bigint", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    weeks_ahead = table.Column<int>(type: "int", nullable: false),
                    generated_until = table.Column<DateOnly>(type: "date", nullable: false),
                    discount_percent = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recurring_bookings", x => x.id);
                    table.CheckConstraint("CK_recurring_bookings_weekday", "[weekday] BETWEEN 1 AND 7");
                    table.ForeignKey(
                        name: "fk_recurring_bookings_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_recurring_bookings_courts_court_id",
                        column: x => x.court_id,
                        principalTable: "courts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_recurring_bookings_time_slots_slot_id",
                        column: x => x.slot_id,
                        principalTable: "time_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_recurring_bookings_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    branch_id = table.Column<long>(type: "bigint", nullable: false),
                    court_id = table.Column<long>(type: "bigint", nullable: false),
                    rating = table.Column<byte>(type: "tinyint", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reviews", x => x.id);
                    table.CheckConstraint("CK_reviews_rating", "[rating] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "fk_reviews_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reviews_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reviews_courts_court_id",
                        column: x => x.court_id,
                        principalTable: "courts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reviews_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "system_settings",
                columns: table => new
                {
                    key = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    value = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: false),
                    is_encrypted = table.Column<bool>(type: "bit", nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    updated_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_settings", x => x.key);
                    table.ForeignKey(
                        name: "fk_system_settings_users_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "waitlist",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    court_id = table.Column<long>(type: "bigint", nullable: false),
                    slot_date = table.Column<DateOnly>(type: "date", nullable: false),
                    slot_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    notified_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_waitlist", x => x.id);
                    table.ForeignKey(
                        name: "fk_waitlist_courts_court_id",
                        column: x => x.court_id,
                        principalTable: "courts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_waitlist_time_slots_slot_id",
                        column: x => x.slot_id,
                        principalTable: "time_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_waitlist_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "incidents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    court_id = table.Column<long>(type: "bigint", nullable: false),
                    equipment_id = table.Column<long>(type: "bigint", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: false),
                    severity = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    image_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    reported_by_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    set_maintenance = table.Column<bool>(type: "bit", nullable: false),
                    resolved_note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    resolved_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_incidents", x => x.id);
                    table.ForeignKey(
                        name: "fk_incidents_courts_court_id",
                        column: x => x.court_id,
                        principalTable: "courts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_incidents_equipment_equipment_id",
                        column: x => x.equipment_id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "slot_locks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    court_id = table.Column<long>(type: "bigint", nullable: false),
                    slot_date = table.Column<DateOnly>(type: "date", nullable: false),
                    slot_id = table.Column<long>(type: "bigint", nullable: false),
                    source_type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    booking_slot_id = table.Column<long>(type: "bigint", nullable: true),
                    block_id = table.Column<long>(type: "bigint", nullable: true),
                    locked_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_slot_locks", x => x.id);
                    table.CheckConstraint("CK_slot_locks_source", "([source_type] = 'Booking' AND [booking_slot_id] IS NOT NULL AND [block_id] IS NULL) OR ([source_type] = 'Block' AND [block_id] IS NOT NULL AND [booking_slot_id] IS NULL)");
                    table.ForeignKey(
                        name: "fk_slot_locks_booking_slots_booking_slot_id",
                        column: x => x.booking_slot_id,
                        principalTable: "booking_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_slot_locks_courts_court_id",
                        column: x => x.court_id,
                        principalTable: "courts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_slot_locks_slot_blocks_block_id",
                        column: x => x.block_id,
                        principalTable: "slot_blocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_slot_locks_time_slots_slot_id",
                        column: x => x.slot_id,
                        principalTable: "time_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chat_messages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    thread_id = table.Column<long>(type: "bigint", nullable: false),
                    sender_type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    sender_id = table.Column<long>(type: "bigint", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: false),
                    meta = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    read_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_chat_messages_chat_threads_thread_id",
                        column: x => x.thread_id,
                        principalTable: "chat_threads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_chat_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "complaint_messages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    complaint_id = table.Column<long>(type: "bigint", nullable: false),
                    sender_id = table.Column<long>(type: "bigint", nullable: false),
                    sender_role = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: false),
                    attachment_urls = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_complaint_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_complaint_messages_complaints_complaint_id",
                        column: x => x.complaint_id,
                        principalTable: "complaints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_complaint_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recurring_skips",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recurring_id = table.Column<long>(type: "bigint", nullable: false),
                    skip_date = table.Column<DateOnly>(type: "date", nullable: false),
                    reason = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recurring_skips", x => x.id);
                    table.ForeignKey(
                        name: "fk_recurring_skips_recurring_bookings_recurring_id",
                        column: x => x.recurring_id,
                        principalTable: "recurring_bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "review_replies",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    review_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_review_replies", x => x.id);
                    table.ForeignKey(
                        name: "fk_review_replies_reviews_review_id",
                        column: x => x.review_id,
                        principalTable: "reviews",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_review_replies_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_actor_user_id",
                table: "audit_logs",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_branch_id_created_at",
                table: "audit_logs",
                columns: new[] { "branch_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_created_at",
                table: "audit_logs",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_auth_tokens_user_id",
                table: "auth_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_history_booking_id",
                table: "booking_history",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_history_performed_by_user_id",
                table: "booking_history",
                column: "performed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_services_booking_id",
                table: "booking_services",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_services_service_id",
                table: "booking_services",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_slots_booking_id_court_id_slot_date_slot_id",
                table: "booking_slots",
                columns: new[] { "booking_id", "court_id", "slot_date", "slot_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_booking_slots_court_id_slot_date",
                table: "booking_slots",
                columns: new[] { "court_id", "slot_date" });

            migrationBuilder.CreateIndex(
                name: "ix_booking_slots_slot_id",
                table: "booking_slots",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_branch_id_play_date",
                table: "bookings",
                columns: new[] { "branch_id", "play_date" });

            migrationBuilder.CreateIndex(
                name: "ix_bookings_cancelled_by_user_id",
                table: "bookings",
                column: "cancelled_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_code",
                table: "bookings",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bookings_created_by_user_id",
                table: "bookings",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_customer_id",
                table: "bookings",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_recurring_id",
                table: "bookings",
                column: "recurring_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_status_hold_expires_at",
                table: "bookings",
                columns: new[] { "status", "hold_expires_at" });

            migrationBuilder.CreateIndex(
                name: "ix_bookings_status_start_at",
                table: "bookings",
                columns: new[] { "status", "start_at" });

            migrationBuilder.CreateIndex(
                name: "ix_bookings_voucher_id",
                table: "bookings",
                column: "voucher_id");

            migrationBuilder.CreateIndex(
                name: "ix_branch_amenities_amenity_id",
                table: "branch_amenities",
                column: "amenity_id");

            migrationBuilder.CreateIndex(
                name: "ix_branch_images_branch_id",
                table: "branch_images",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_branch_policies_branch_id",
                table: "branch_policies",
                column: "branch_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_branch_staff_names_branch_id_is_active",
                table: "branch_staff_names",
                columns: new[] { "branch_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_branches_approved_by_user_id",
                table: "branches",
                column: "approved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_branches_owner_user_id_name",
                table: "branches",
                columns: new[] { "owner_user_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_broadcasts_sent_by_user_id",
                table: "broadcasts",
                column: "sent_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_cancel_policy_tiers_branch_id_hours_before",
                table: "cancel_policy_tiers",
                columns: new[] { "branch_id", "hours_before" },
                unique: true,
                filter: "[branch_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_chat_messages_sender_id",
                table: "chat_messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_messages_thread_id_created_at",
                table: "chat_messages",
                columns: new[] { "thread_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_chat_threads_branch_id",
                table: "chat_threads",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_threads_customer_id",
                table: "chat_threads",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_complaint_messages_complaint_id",
                table: "complaint_messages",
                column: "complaint_id");

            migrationBuilder.CreateIndex(
                name: "ix_complaint_messages_sender_id",
                table: "complaint_messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_complaints_booking_id",
                table: "complaints",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "ix_complaints_branch_id",
                table: "complaints",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_complaints_code",
                table: "complaints",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_complaints_customer_id",
                table: "complaints",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_complaints_resolved_by_user_id",
                table: "complaints",
                column: "resolved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_court_types_name",
                table: "court_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_courts_branch_id_name",
                table: "courts",
                columns: new[] { "branch_id", "name" },
                unique: true,
                filter: "[deleted_at] IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_courts_court_type_id",
                table: "courts",
                column: "court_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_violations_booking_id",
                table: "customer_violations",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_violations_resolved_by_user_id",
                table: "customer_violations",
                column: "resolved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_violations_user_id",
                table: "customer_violations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_daily_closings_branch_id_closing_date",
                table: "daily_closings",
                columns: new[] { "branch_id", "closing_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_equipment_court_id",
                table: "equipment",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "ix_favorites_branch_id",
                table: "favorites",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_favorites_court_id",
                table: "favorites",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "ix_favorites_customer_id_branch_id_court_id",
                table: "favorites",
                columns: new[] { "customer_id", "branch_id", "court_id" },
                unique: true,
                filter: "[court_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_holidays_branch_id_holiday_date",
                table: "holidays",
                columns: new[] { "branch_id", "holiday_date" },
                unique: true,
                filter: "[branch_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_incidents_court_id",
                table: "incidents",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "ix_incidents_equipment_id",
                table: "incidents",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_items_invoice_id",
                table: "invoice_items",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_booking_id",
                table: "invoices",
                column: "booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoices_invoice_no",
                table: "invoices",
                column: "invoice_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_id_is_read",
                table: "notifications",
                columns: new[] { "user_id", "is_read" });

            migrationBuilder.CreateIndex(
                name: "ix_payments_booking_id",
                table: "payments",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_gateway_txn_id",
                table: "payments",
                column: "gateway_txn_id",
                unique: true,
                filter: "[gateway_txn_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_payments_related_payment_id",
                table: "payments",
                column: "related_payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_price_rules_branch_id_court_type_id_court_id_day_type_slot_type_valid_from",
                table: "price_rules",
                columns: new[] { "branch_id", "court_type_id", "court_id", "day_type", "slot_type", "valid_from" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_price_rules_court_id",
                table: "price_rules",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "ix_price_rules_court_type_id",
                table: "price_rules",
                column: "court_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_recurring_bookings_branch_id",
                table: "recurring_bookings",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_recurring_bookings_court_id",
                table: "recurring_bookings",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "ix_recurring_bookings_customer_id_court_id_weekday_slot_id",
                table: "recurring_bookings",
                columns: new[] { "customer_id", "court_id", "weekday", "slot_id" },
                unique: true,
                filter: "[status] = 'Active'");

            migrationBuilder.CreateIndex(
                name: "ix_recurring_bookings_slot_id",
                table: "recurring_bookings",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "ix_recurring_skips_recurring_id_skip_date",
                table: "recurring_skips",
                columns: new[] { "recurring_id", "skip_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_review_replies_review_id",
                table: "review_replies",
                column: "review_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_review_replies_user_id",
                table: "review_replies",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_reviews_booking_id",
                table: "reviews",
                column: "booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reviews_branch_id",
                table: "reviews",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_reviews_court_id",
                table: "reviews",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "ix_reviews_customer_id",
                table: "reviews",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_code",
                table: "roles",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_services_branch_id_name",
                table: "services",
                columns: new[] { "branch_id", "name" },
                unique: true,
                filter: "[deleted_at] IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_slot_blocks_court_id",
                table: "slot_blocks",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "ix_slot_blocks_end_slot_id",
                table: "slot_blocks",
                column: "end_slot_id");

            migrationBuilder.CreateIndex(
                name: "ix_slot_blocks_start_slot_id",
                table: "slot_blocks",
                column: "start_slot_id");

            migrationBuilder.CreateIndex(
                name: "ix_slot_locks_block_id",
                table: "slot_locks",
                column: "block_id");

            migrationBuilder.CreateIndex(
                name: "ix_slot_locks_booking_slot_id",
                table: "slot_locks",
                column: "booking_slot_id",
                unique: true,
                filter: "[booking_slot_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_slot_locks_court_id_slot_date_slot_id",
                table: "slot_locks",
                columns: new[] { "court_id", "slot_date", "slot_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_slot_locks_slot_id",
                table: "slot_locks",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_settings_updated_by_user_id",
                table: "system_settings",
                column: "updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_time_slots_branch_id_slot_index",
                table: "time_slots",
                columns: new[] { "branch_id", "slot_index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_time_slots_branch_id_start_time",
                table: "time_slots",
                columns: new[] { "branch_id", "start_time" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_branch_id",
                table: "users",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_phone",
                table: "users",
                column: "phone",
                unique: true,
                filter: "[phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_vouchers_branch_id",
                table: "vouchers",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_vouchers_code",
                table: "vouchers",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_court_id",
                table: "waitlist",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_customer_id_court_id_slot_date_slot_id",
                table: "waitlist",
                columns: new[] { "customer_id", "court_id", "slot_date", "slot_id" },
                unique: true,
                filter: "[status] = 'Waiting'");

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_slot_id",
                table: "waitlist",
                column: "slot_id");

            migrationBuilder.AddForeignKey(
                name: "fk_audit_logs_branches_branch_id",
                table: "audit_logs",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_audit_logs_users_actor_user_id",
                table: "audit_logs",
                column: "actor_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_auth_tokens_users_user_id",
                table: "auth_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_booking_history_bookings_booking_id",
                table: "booking_history",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_booking_history_users_performed_by_user_id",
                table: "booking_history",
                column: "performed_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_booking_reminders_bookings_booking_id",
                table: "booking_reminders",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_booking_services_bookings_booking_id",
                table: "booking_services",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_booking_services_services_service_id",
                table: "booking_services",
                column: "service_id",
                principalTable: "services",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_booking_slots_bookings_booking_id",
                table: "booking_slots",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_booking_slots_courts_court_id",
                table: "booking_slots",
                column: "court_id",
                principalTable: "courts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_booking_slots_time_slots_slot_id",
                table: "booking_slots",
                column: "slot_id",
                principalTable: "time_slots",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_branches_branch_id",
                table: "bookings",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_recurring_bookings_recurring_id",
                table: "bookings",
                column: "recurring_id",
                principalTable: "recurring_bookings",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_users_cancelled_by_user_id",
                table: "bookings",
                column: "cancelled_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_users_created_by_user_id",
                table: "bookings",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_users_customer_id",
                table: "bookings",
                column: "customer_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_vouchers_voucher_id",
                table: "bookings",
                column: "voucher_id",
                principalTable: "vouchers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_branch_amenities_branches_branch_id",
                table: "branch_amenities",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_branch_images_branches_branch_id",
                table: "branch_images",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_branch_policies_branches_branch_id",
                table: "branch_policies",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_branch_staff_names_branches_branch_id",
                table: "branch_staff_names",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_branches_users_approved_by_user_id",
                table: "branches",
                column: "approved_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_branches_users_owner_user_id",
                table: "branches",
                column: "owner_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_branches_branch_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "auth_tokens");

            migrationBuilder.DropTable(
                name: "booking_history");

            migrationBuilder.DropTable(
                name: "booking_reminders");

            migrationBuilder.DropTable(
                name: "booking_services");

            migrationBuilder.DropTable(
                name: "branch_amenities");

            migrationBuilder.DropTable(
                name: "branch_images");

            migrationBuilder.DropTable(
                name: "branch_policies");

            migrationBuilder.DropTable(
                name: "branch_staff_names");

            migrationBuilder.DropTable(
                name: "broadcasts");

            migrationBuilder.DropTable(
                name: "cancel_policy_tiers");

            migrationBuilder.DropTable(
                name: "chat_messages");

            migrationBuilder.DropTable(
                name: "complaint_messages");

            migrationBuilder.DropTable(
                name: "customer_violations");

            migrationBuilder.DropTable(
                name: "daily_closings");

            migrationBuilder.DropTable(
                name: "email_queue");

            migrationBuilder.DropTable(
                name: "favorites");

            migrationBuilder.DropTable(
                name: "holidays");

            migrationBuilder.DropTable(
                name: "incidents");

            migrationBuilder.DropTable(
                name: "invoice_items");

            migrationBuilder.DropTable(
                name: "job_runs");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "price_rules");

            migrationBuilder.DropTable(
                name: "recurring_skips");

            migrationBuilder.DropTable(
                name: "review_replies");

            migrationBuilder.DropTable(
                name: "slot_locks");

            migrationBuilder.DropTable(
                name: "system_settings");

            migrationBuilder.DropTable(
                name: "waitlist");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "amenities");

            migrationBuilder.DropTable(
                name: "chat_threads");

            migrationBuilder.DropTable(
                name: "complaints");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "invoices");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "booking_slots");

            migrationBuilder.DropTable(
                name: "slot_blocks");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "recurring_bookings");

            migrationBuilder.DropTable(
                name: "vouchers");

            migrationBuilder.DropTable(
                name: "courts");

            migrationBuilder.DropTable(
                name: "time_slots");

            migrationBuilder.DropTable(
                name: "court_types");

            migrationBuilder.DropTable(
                name: "branches");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
