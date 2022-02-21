using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetPatientTimestampOnUpdate
                    AFTER UPDATE ON Patients
                    BEGIN
                        UPDATE Patients
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetPatientTimestampOnInsert
                    AFTER INSERT ON Patients
                    BEGIN
                        UPDATE Patients
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

            migrationBuilder.Sql(
                @"
                    Create View AppointmentSummaries as
                    Select p.ID, p.FirstName, p.MiddleName, p.LastName, Count(*) as NumberOfAppointments, 
	                    Sum(a.extraFee) as TotalExtraFees, Max(a.extraFee) as MaximumFeeCharged
                    From Patients p Join Appointments a
                    on p.ID = a.PatientID
                    Group By p.ID, p.FirstName, p.MiddleName, p.LastName
                ");

            migrationBuilder.Sql(
                @"
                    CREATE VIEW UserFullNameView as
                    SELECT u.Id, u.UserName, e.FirstName || ' ' || e.LastName as UserFullName
                    From AspNetUsers u Left Join Employees e
                    on u.Email = e.Email;
                ");

            migrationBuilder.Sql(
                @"
                    CREATE VIEW RolesWithUsers as
                    Select r.Name as RoleName, u.UserName, u.UserFullName
                    From AspNetRoles r Left Join AspNetUserRoles ur
                    on r.Id = ur.RoleId left join UserFullNameView u
                    on u.Id = ur.UserId;
                ");
        }
    }
}
