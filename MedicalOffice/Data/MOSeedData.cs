using MedicalOffice.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Data
{
    public static class MOSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new MedicalOfficeContext(
                serviceProvider.GetRequiredService<DbContextOptions<MedicalOfficeContext>>()))
            {
                //Provinces, Cities 
                if (!context.Provinces.Any())
                {
                    var provinces = new List<Province>
                    {
                        new Province { Code = "ON", Name = "Ontario"},
                        new Province { Code = "PE", Name = "Prince Edward Island"},
                        new Province { Code = "NB", Name = "New Brunswick"},
                        new Province { Code = "BC", Name = "British Columbia"},
                        new Province { Code = "NL", Name = "Newfoundland and Labrador"},
                        new Province { Code = "SK", Name = "Saskatchewan"},
                        new Province { Code = "NS", Name = "Nova Scotia"},
                        new Province { Code = "MB", Name = "Manitoba"},
                        new Province { Code = "QC", Name = "Quebec"},
                        new Province { Code = "YT", Name = "Yukon"},
                        new Province { Code = "NU", Name = "Nunavut"},
                        new Province { Code = "NT", Name = "Northwest Territories"},
                        new Province { Code = "AB", Name = "Alberta"}
                    };
                    context.Provinces.AddRange(provinces);
                    context.SaveChanges();
                }
                if (!context.Cities.Any())
                {
                    var cities = new List<City>
                    {
                        new City { Name = "Toronto", ProvinceID=context.Provinces.FirstOrDefault(d => d.Name == "Ontario").ID, },
                        new City { Name = "Halifax", ProvinceID=context.Provinces.FirstOrDefault(d => d.Name == "Nova Scotia").ID, },
                        new City { Name = "Calgary", ProvinceID=context.Provinces.FirstOrDefault(d => d.Name == "Alberta").ID, },
                        new City { Name = "Winnipeg", ProvinceID=context.Provinces.FirstOrDefault(d => d.Name == "Manitoba").ID, },
                        new City { Name = "Stratford", ProvinceID=context.Provinces.FirstOrDefault(d => d.Name == "Ontario").ID, },
                        new City { Name = "St. Catharines", ProvinceID=context.Provinces.FirstOrDefault(d => d.Name == "Ontario").ID, },
                        new City { Name = "Stratford", ProvinceID=context.Provinces.FirstOrDefault(d => d.Name == "Prince Edward Island").ID, },
                        new City { Name = "Ancaster", ProvinceID=context.Provinces.FirstOrDefault(d => d.Name == "Ontario").ID, },
                        new City { Name = "Vancouver", ProvinceID=context.Provinces.FirstOrDefault(d => d.Name == "British Columbia").ID, },
                    };
                    context.Cities.AddRange(cities);
                    context.SaveChanges();
                }

                // Look for any Patients.  Since we can't have patients without Doctors.
                if (!context.Doctors.Any())
                {
                    context.Doctors.AddRange(
                     new Doctor
                     {
                         FirstName = "Gregory",
                         MiddleName = "A",
                         LastName = "House"
                     },

                     new Doctor
                     {
                         FirstName = "Doogie",
                         MiddleName = "R",
                         LastName = "Houser"
                     },
                     new Doctor
                     {
                         FirstName = "Charles",
                         LastName = "Xavier"
                     }
                );
                    context.SaveChanges();
                }
                //Add more Doctors
                if (context.Doctors.Count() < 4)
                {
                    string[] firstNames = new string[] { "Woodstock", "Violet", "Charlie", "Lucy", "Linus", "Franklin", "Marcie", "Schroeder" };
                    string[] lastNames = new string[] { "Hightower", "Broomspun", "Jones", "Bloggs", "Brown", "Smith", "Daniel" };

                    //Loop through names and add more
                    foreach (string lastName in lastNames)
                    {
                        foreach (string firstname in firstNames)
                        {
                            //Construct some details
                            Doctor a = new Doctor()
                            {
                                FirstName = firstname,
                                LastName = lastName,
                                MiddleName = lastName[1].ToString().ToUpper(),
                            };
                            context.Doctors.Add(a);
                        }
                    }
                    context.SaveChanges();
                }

                //Add some Medical Trials
                if (!context.MedicalTrials.Any())
                {
                    context.MedicalTrials.AddRange(
                     new MedicalTrial
                     {
                         TrialName = "UOT - Lukemia Treatment"
                     }, new MedicalTrial
                     {
                         TrialName = "HyGIeaCare Center -  Microbiome Analysis of Constipated Versus Non-constipation Patients"
                     }, new MedicalTrial
                     {
                         TrialName = "Sunnybrook -  Trial of BNT162b2 versus mRNA-1273 COVID-19 Vaccine Boosters in Chronic Kidney Disease and Dialysis Patients With Poor Humoral Response following COVID-19 Vaccination"
                     }, new MedicalTrial
                     {
                         TrialName = "Altimmune -  Evaluate the Effect of Position and Duration on the Safety and Immunogenicity of Intranasal AdCOVID Administration"
                     }, new MedicalTrial
                     {
                         TrialName = "HyGIeaCare Center -  Microbiome Analysis of Constipated Versus Non-constipation Patients"
                     }, new MedicalTrial
                     {
                         TrialName = "TUK - Hair Loss Treatment"
                     });
                    context.SaveChanges();
                }

                //Add some Specific Patients
                if (!context.Patients.Any())
                {
                    context.Patients.AddRange(
                    new Patient
                    {
                        FirstName = "Fred",
                        MiddleName = "Reginald",
                        LastName = "Flintstone",
                        OHIP = "1231231234",
                        DOB = DateTime.Parse("1955-09-01"),
                        ExpYrVisits = 6,
                        Phone = "9055551212",
                        EMail = "fflintstone@outlook.com",
                        DoctorID = context.Doctors.FirstOrDefault(d => d.FirstName == "Gregory" && d.LastName == "House").ID,
                        MedicalTrialID = context.MedicalTrials.FirstOrDefault(d => d.TrialName.Contains("UOT")).ID
                    },
                    new Patient
                    {
                        FirstName = "Wilma",
                        MiddleName = "Jane",
                        LastName = "Flintstone",
                        OHIP = "1321321324",
                        DOB = DateTime.Parse("1964-04-23"),
                        ExpYrVisits = 2,
                        Phone = "9055551212",
                        EMail = "wflintstone@outlook.com",
                        DoctorID = context.Doctors.FirstOrDefault(d => d.FirstName == "Gregory" && d.LastName == "House").ID
                    },
                    new Patient
                    {
                        FirstName = "Barney",
                        LastName = "Rubble",
                        OHIP = "3213213214",
                        DOB = DateTime.Parse("1964-02-22"),
                        ExpYrVisits = 2,
                        Phone = "9055551213",
                        EMail = "brubble@outlook.com",
                        DoctorID = context.Doctors.FirstOrDefault(d => d.FirstName == "Doogie" && d.LastName == "Houser").ID,
                        MedicalTrialID = context.MedicalTrials.FirstOrDefault(d => d.TrialName.Contains("UOT")).ID
                    },
                    new Patient
                    {
                        FirstName = "Jane",
                        MiddleName = "Samantha",
                        LastName = "Doe",
                        OHIP = "4124124123",
                        ExpYrVisits = 2,
                        Phone = "9055551234",
                        EMail = "jdoe@outlook.com",
                        DoctorID = context.Doctors.FirstOrDefault(d => d.FirstName == "Charles" && d.LastName == "Xavier").ID
                    });
                    context.SaveChanges();
                }

                //To randomly generate data
                Random random = new Random();

                //Create collections of the primary keys
                int[] doctorIDs = context.Doctors.Select(a => a.ID).ToArray();
                int doctorIDCount = doctorIDs.Count();// Why does this help efficiency?
                int[] medicalTrialIDs = context.MedicalTrials.Select(a => a.ID).ToArray();
                int medicalTrialIDCount = medicalTrialIDs.Count();

                //More Patients.  Now it gets more interesting becuase we
                //have Foreign Keys to worry about
                //and more complicated property values to generate
                if (context.Patients.Count() < 6)
                {
                   
                    string[] firstNames = new string[] { "Lyric", "Antoinette", "Kendal", "Vivian", "Ruth", "Jamison", "Emilia", "Natalee", "Yadiel", "Jakayla", "Lukas", "Moses", "Kyler", "Karla", "Chanel", "Tyler", "Camilla", "Quintin", "Braden", "Clarence" };
                    string[] lastNames = new string[] { "Watts", "Randall", "Arias", "Weber", "Stone", "Carlson", "Robles", "Frederick", "Parker", "Morris", "Soto", "Bruce", "Orozco", "Boyer", "Burns", "Cobb", "Blankenship", "Houston", "Estes", "Atkins", "Miranda", "Zuniga", "Ward", "Mayo", "Costa", "Reeves", "Anthony", "Cook", "Krueger", "Crane", "Watts", "Little", "Henderson", "Bishop" };
                    int firstNameCount = firstNames.Count();

                    // Birthdate for randomly produced Patients 
                    // We will subtract a random number of days from today
                    DateTime startDOB = DateTime.Today;// More efficiency?
                    int counter = 1; //Used to help add some Patients to Medical Trials

                    foreach (string lastName in lastNames)
                    {
                        //Choose a random HashSet of 4 (Unique) first names
                        HashSet<string> selectedFirstNames = new HashSet<string>();
                        while (selectedFirstNames.Count() < 4)
                        {
                            selectedFirstNames.Add(firstNames[random.Next(firstNameCount)]);
                        }

                        foreach (string firstName in selectedFirstNames)
                        {
                            //Construct some Patient details
                            Patient patient = new Patient()
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                MiddleName = lastName[1].ToString().ToUpper(),
                                OHIP = random.Next(2, 9).ToString() + random.Next(213214131, 989898989).ToString(),
                                EMail = (firstName.Substring(0, 2) + lastName + random.Next(11, 111).ToString() + "@outlook.com").ToLower(),
                                Phone = random.Next(2, 10).ToString() + random.Next(213214131, 989898989).ToString(),
                                ExpYrVisits = (byte)random.Next(2, 12),
                                DOB = startDOB.AddDays(-random.Next(60, 34675)),
                                DoctorID = doctorIDs[random.Next(doctorIDCount)]
                            };
                            if (counter % 3 == 0)//Every third Patient gets assigned to a Medical Trial
                            {
                                patient.MedicalTrialID = medicalTrialIDs[random.Next(medicalTrialIDCount)];
                            }
                            counter++;
                            context.Patients.Add(patient);
                            try
                            {
                                //Could be a duplicate OHIP
                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                    //Since we didn't guarantee that evey Doctor had
                    //at least one Patient assigned, let's remove Doctors
                    //without any Patients.  We could do this other ways, but it
                    //gives a chance to show how to execute 
                    //raw SQL through our DbContext.
                    string cmd = "DELETE FROM Doctors WHERE NOT EXISTS(SELECT 1 FROM Patients WHERE Doctors.Id = Patients.DoctorID)";
                    context.Database.ExecuteSqlRaw(cmd);
                }
                //Since we have removed Doctors, we need to refresh the collection
                //of Primary Keys for Doctors
                doctorIDs = context.Doctors.Select(a => a.ID).ToArray();
                doctorIDCount = doctorIDs.Count();

                //Conditions
                if (!context.Conditions.Any())
                {
                    string[] conditions = new string[] { "Asthma", "Cancer", "Cardiac disease", "Diabetes", "Hypertension", "Seizure disorder", "Circulation problems", "Bleeding disorder", "Thyroid condition", "Liver Disease", "Measles", "Mumps" };

                    foreach (string condition in conditions)
                    {
                        Condition c = new Condition
                        {
                            ConditionName = condition
                        };
                        context.Conditions.Add(c);
                    }
                    context.SaveChanges();
                }

                //PatientConditions
                if (!context.PatientConditions.Any())
                {
                    context.PatientConditions.AddRange(
                        new PatientCondition
                        {
                            ConditionID = context.Conditions.FirstOrDefault(c => c.ConditionName == "Cancer").ID,
                            PatientID = context.Patients.FirstOrDefault(p => p.LastName == "Flintstone" && p.FirstName == "Fred").ID
                        },
                        new PatientCondition
                        {
                            ConditionID = context.Conditions.FirstOrDefault(c => c.ConditionName == "Diabetes").ID,
                            PatientID = context.Patients.FirstOrDefault(p => p.LastName == "Flintstone" && p.FirstName == "Fred").ID
                        },
                        new PatientCondition
                        {
                            ConditionID = context.Conditions.FirstOrDefault(c => c.ConditionName == "Hypertension").ID,
                            PatientID = context.Patients.FirstOrDefault(p => p.LastName == "Flintstone" && p.FirstName == "Wilma").ID
                        });
                    context.SaveChanges();
                }
                
                //Seed Specialties
                string[] specialties = new string[] { "Abdominal Radiology", "Addiction Psychiatry", "Adolescent Medicine Pediatrics", "Cardiothoracic Anesthesiology", "Adult Reconstructive Orthopaedics", "Advanced Heart Failure ", "Allergy & Immunology ", "Anesthesiology ", "Biochemical Genetics", "Blood Banking ", "Cardiothoracic Radiology", "Cardiovascular Disease Internal Medicine", "Chemical Pathology", "Child & Adolescent Psychiatry", "Child Abuse Pediatrics", "Child Neurology", "Clinical & Laboratory Immunology", "Clinical Cardiac Electrophysiology", "Clinical Neurophysiology Neurology", "Colon & Rectal Surgery ", "Congenital Cardiac Surgery", "Craniofacial Surgery", "Critical Care Medicine", "Cytopathology ", "Dermatology ", "Dermatopathology ", "Family Medicine ", "Family Practice", "Female Pelvic Medicine", "Foot & Ankle Orthopaedics", "Forensic Pathology", "Forensic Psychiatry ", "Hand Surgery", "Hematology Pathology", "Oncology ", "Infectious Disease", "Internal Medicine ", "Interventional Cardiology", "Neonatal-Perinatal Medicine", "Nephrology Internal Medicine", "Neurological Surgery ", "Neurology ", "Neuromuscular Medicine", "Neuropathology Pathology", "Nuclear Medicine ", "Nuclear Radiology", "Obstetric Anesthesiology", "Obstetrics & Gynecology ", "Ophthalmic Plastic", "Ophthalmology ", "Orthopaedic Sports Medicine", "Orthopaedic Surgery ", "Otolaryngology ", "Otology", "Pediatrics ", "Plastic Surgery ", "Preventive Medicine ", "Radiation Oncology ", "Rheumatology", "Vascular & Interventional Radiology", "Vascular Surgery", "Integrated Thoracic Surgery", "Transplant Hepatology", "Urology" };
                if (!context.Specialties.Any())
                {
                    foreach (string s in specialties)
                    {
                        Specialty sp = new Specialty
                        {
                            SpecialtyName = s
                        };
                        context.Specialties.Add(sp);
                    }
                    context.SaveChanges();
                }
                
                //Create collection of the primary keys of the Specialties
                int[] specialtyIDs = context.Specialties.Select(s => s.ID).ToArray();
                int specialtyCount = specialtyIDs.Count();

                //DoctorSpecialties - the Intersection
                //Add a specialty to each Doctor
                if (!context.DoctorSpecialties.Any())
                {
                    foreach (int i in doctorIDs)
                    {
                        int specialtyID = random.Next(1,specialtyCount-2);
                        var doctorSpecialties = new List<DoctorSpecialty>
                        {
                            new DoctorSpecialty { DoctorID = i, SpecialtyID = specialtyID},
                            new DoctorSpecialty { DoctorID = i, SpecialtyID = specialtyID+1},
                            new DoctorSpecialty { DoctorID = i, SpecialtyID = specialtyID+2}
                        };
                        context.DoctorSpecialties.AddRange(doctorSpecialties);
                        context.SaveChanges();
                    }
                }

                //Seed Appointment Reasons
                string[] AppointmentReasons = new string[] { "Illness", "Accident", "Mental State", "Annual Checkup", "COVID-19" };
                if (!context.AppointmentReasons.Any())
                {
                    foreach (string s in AppointmentReasons)
                    {
                        AppointmentReason ar = new AppointmentReason
                        {
                            ReasonName = s
                        };
                        context.AppointmentReasons.Add(ar);
                    }
                    context.SaveChanges();
                }

                //Create 5 notes from Bacon ipsum
                string[] baconNotes = new string[] { "Bacon ipsum dolor amet meatball corned beef kevin, alcatra kielbasa biltong drumstick strip steak spare ribs swine. Pastrami shank swine leberkas bresaola, prosciutto frankfurter porchetta ham hock short ribs short loin andouille alcatra. Andouille shank meatball pig venison shankle ground round sausage kielbasa. Chicken pig meatloaf fatback leberkas venison tri-tip burgdoggen tail chuck sausage kevin shank biltong brisket.", "Sirloin shank t-bone capicola strip steak salami, hamburger kielbasa burgdoggen jerky swine andouille rump picanha. Sirloin porchetta ribeye fatback, meatball leberkas swine pancetta beef shoulder pastrami capicola salami chicken. Bacon cow corned beef pastrami venison biltong frankfurter short ribs chicken beef. Burgdoggen shank pig, ground round brisket tail beef ribs turkey spare ribs tenderloin shankle ham rump. Doner alcatra pork chop leberkas spare ribs hamburger t-bone. Boudin filet mignon bacon andouille, shankle pork t-bone landjaeger. Rump pork loin bresaola prosciutto pancetta venison, cow flank sirloin sausage.", "Porchetta pork belly swine filet mignon jowl turducken salami boudin pastrami jerky spare ribs short ribs sausage andouille. Turducken flank ribeye boudin corned beef burgdoggen. Prosciutto pancetta sirloin rump shankle ball tip filet mignon corned beef frankfurter biltong drumstick chicken swine bacon shank. Buffalo kevin andouille porchetta short ribs cow, ham hock pork belly drumstick pastrami capicola picanha venison.", "Picanha andouille salami, porchetta beef ribs t-bone drumstick. Frankfurter tail landjaeger, shank kevin pig drumstick beef bresaola cow. Corned beef pork belly tri-tip, ham drumstick hamburger swine spare ribs short loin cupim flank tongue beef filet mignon cow. Ham hock chicken turducken doner brisket. Strip steak cow beef, kielbasa leberkas swine tongue bacon burgdoggen beef ribs pork chop tenderloin.", "Kielbasa porchetta shoulder boudin, pork strip steak brisket prosciutto t-bone tail. Doner pork loin pork ribeye, drumstick brisket biltong boudin burgdoggen t-bone frankfurter. Flank burgdoggen doner, boudin porchetta andouille landjaeger ham hock capicola pork chop bacon. Landjaeger turducken ribeye leberkas pork loin corned beef. Corned beef turducken landjaeger pig bresaola t-bone bacon andouille meatball beef ribs doner. T-bone fatback cupim chuck beef ribs shank tail strip steak bacon." };

                //Create collections of the primary keys of the two Parents
                int[] AppointmentReasonIDs = context.AppointmentReasons.Select(s => s.ID).ToArray();
                int[] patientIDs = context.Patients.Select(d => d.ID).ToArray();

                //Appointments - the Intersection
                //Add a few appointments to each patient
                if (!context.Appointments.Any())
                {
                    int reasonCount = AppointmentReasonIDs.Count();
                    foreach (int i in patientIDs)
                    {
                        int howMany = random.Next(1, 4);
                        for (int j = 1; j <= howMany; j++)
                        {
                            Appointment a = new Appointment()
                            {
                                PatientID = i,
                                AppointmentReasonID = AppointmentReasonIDs[random.Next(reasonCount)],
                                AppointmentDate = DateTime.Today.AddDays(-1 * random.Next(120)),
                                Notes = baconNotes[random.Next(5)]
                            };
                            context.Appointments.Add(a);
                            try
                            {
                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                }


            }
        }
    }
}
