using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using IncidentUtility.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Timer = System.Threading.Timer;
using IncidentUtility.Services;


namespace IncidentUtility
{
    public partial class Form1 : Form
    {
        private RenjiDbContext db = new RenjiDbContext();
        private EmailNotification emailNotification = new EmailNotification();
        private Timer? incidentMonitoringTimer; // Declaring timer as a type of Timer Class (Field Declaration)
        private Timer? ActionPlanMonitoringTimer;
        private Timer? OfficerResponseMonitoringTimer;
        private Timer? CheckDueActionPlanTimer;
        private Timer? DueActionPlanMonitoringTimer;
        private System.Windows.Forms.Timer? displayTimer;
        private int countdown = 15;
        private bool _incidentMonitoringRunning = false;
        private bool _actionPlanMonitoringSafe = false;
        private bool _officerResponseMonitoringSafe = false;
        private bool _checkDueActionPlanSafe = false;
        private bool _dueActionPlanMonitoringSafe = false;

        public Form1()
        {
            // This one is running on the main thread
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            dataGridView2.Columns.Clear();
            dataGridView3.Columns.Clear();
            dataGridView4.Columns.Clear();
            dataGridView5.Columns.Clear();

            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView3.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            // Setup Label
            label1.Text = "Incident Monitoring";
            label1.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            label1.TextAlign = ContentAlignment.MiddleCenter;


            label2.Text = "Pending Action Plans";
            label2.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            label2.TextAlign = ContentAlignment.MiddleCenter;

            label3.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            label3.TextAlign = ContentAlignment.MiddleCenter;

            label4.Text = "No of Overdue Plans per Team";
            label4.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            label4.TextAlign = ContentAlignment.MiddleCenter;

            label5.Text = "Priority: Red - High, Yellow - Moderate, White - Low";
            label5.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            label5.TextAlign = ContentAlignment.MiddleCenter;

            label6.Text = "Maintenance Team Contact Number";
            label6.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            label6.TextAlign = ContentAlignment.MiddleCenter;

            label7.Text = "Due Action Plans";
            label7.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            label7.TextAlign = ContentAlignment.MiddleCenter;



            // Define Incident Monitoring Table Columns
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "Id",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accident",
                HeaderText = "Accident",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Location",
                HeaderText = "Location",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ReportedDate",
                HeaderText = "Reported Date",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ReportedBy",
                HeaderText = "Reported By",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Department",
                HeaderText = "Department",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });


            // Define Action Plan Monitoring Table Columns
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "Id",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ActionDetail",
                HeaderText = "Action Plan",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "IncidentTitle",
                HeaderText = "Incident",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DueDate",
                HeaderText = "Due Date",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ActionType",
                HeaderText = "Action Type",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AssignedTeam",
                HeaderText = "Assigned Team",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ContactNumber",
                HeaderText = "Contact Number",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Priority",
                HeaderText = "Priority",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });


            // Define Officer Monitoring Table Columns
            dataGridView3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ResponsibleTeam",
                HeaderText = "Responsible Team",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NumberOfDuePlans",
                HeaderText = "Number Of Due Plans",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            // Display Contact Number of Maintenance Team
            dataGridView4.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Team",
                HeaderText = "Team",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView4.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ContactNumber",
                HeaderText = "Contact Number",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView4.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });


            // Define Due Action Plan Monitoring Table Columns
            dataGridView5.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "Id",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView5.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ActionDetail",
                HeaderText = "Action Plan",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView5.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "IncidentTitle",
                HeaderText = "Incident",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView5.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DueDate",
                HeaderText = "Due Date",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView5.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ActionType",
                HeaderText = "Action Type",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView5.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AssignedTeam",
                HeaderText = "Assigned Team",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView5.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ContactNumber",
                HeaderText = "Contact Number",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dataGridView5.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Priority",
                HeaderText = "Priority",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            // Load once
            await IncidentMonitoring();
            await ActionPlanMonitoring();
            await OfficerResponseMonitoring();
            await DueActionPlanMonitoring();

            // These four are running on .NET Thread pools (Shared Workers running on the background Threads)
            incidentMonitoringTimer = new Timer(async _ => await IncidentMonitoringRunningSafe(), null, 0, 15000);
            ActionPlanMonitoringTimer = new Timer(async _ => await ActionPlanMonitoringSafe(), null, 0, 15000);
            OfficerResponseMonitoringTimer = new Timer(async _ => await OfficerResponseMonitoringSafe(), null, 0, 15000);
            CheckDueActionPlanTimer = new Timer(async _ => await CheckDueActionPlanSafe(), null, 0, 15000);
            DueActionPlanMonitoringTimer = new Timer(async _ => await DueActionPlanMonitoringSafe(), null, 0, 15000);

            // Setup display timer (for clock)
            displayTimer = new System.Windows.Forms.Timer();
            displayTimer.Interval = 1000; // update every 1 second
            displayTimer.Tick += DisplayTimer_Tick;
            displayTimer.Start();

        }

        private void DisplayTimer_Tick(object sender, EventArgs e)
        {
            countdown--;

            if (countdown <= 0)
            {
                countdown = 15; // reset after reaching 0
            }

            label3.Text = $"Refresh in {countdown}s..";
        }


        private async Task IncidentMonitoring()
        {
            using (var db = new RenjiDbContext())
            {
                var incidentList = await db.IncidentReports.
                                   Include(i => i.Accident).
                                   Include(i => i.Department).
                                   Where(u => u.Status == 10).
                                   Select(n => new
                                   {
                                       ID = n.Id,
                                       Title = n.Title,
                                       Location = n.Location,
                                       ReportedDate = n.ReportedDate,
                                       ReportedBy = db.Users.Where(u => u.Id == n.ReportedBy).Select(n => n.LastName).FirstOrDefault(),
                                       Department = n.Department.Name

                                   }).OrderByDescending(o => o.ReportedDate).AsNoTracking().ToListAsync();


                // Load the row values on the Main UI Thread
                // Invoke means calling a method from the Main UI thread
                this.BeginInvoke(() =>
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();

                    foreach (var record in incidentList)
                    {
                        dataGridView1.Rows.Add(
                            record.ID,
                            record.Title,
                            record.Location,
                            record.ReportedDate.ToLocalTime(),
                            record.ReportedBy,
                            record.Department
                        );
                    }
                });
            }
        }


        private async Task IncidentMonitoringRunningSafe()
        {
            if (_incidentMonitoringRunning) return;
            _incidentMonitoringRunning = false;


            try
            {
                await IncidentMonitoring();
            }
            finally
            {
                _incidentMonitoringRunning = false;
            }
        }


        private async Task ActionPlanMonitoring()
        {
            using (var db = new RenjiDbContext())
            {
                var todaysDate = DateTime.UtcNow;

                var actionPlanList = await db.ActionPlans.
                                     Where(u => u.Status != 30). // Filter action plans that are not yet completed
                                     Where(u => u.DueDate > todaysDate).
                                     Select(n => new
                                     {   
                                         Id = n.Id,
                                         ActionDetail = n.ActionDetail,
                                         IncidentTitle = db.IncidentReports.Where(u => u.Id == n.IncidentReportId).Select(n => n.Title).FirstOrDefault(),
                                         DueDate = n.DueDate.Date,
                                         ActionType = n.ActionType == 10 ? "Corrective" :
                                                        n.ActionType == 20 ? "Preventive" :
                                                        n.ActionType == 30 ? "Mitigation" :
                                                        n.ActionType == 40 ? "Containment" :
                                                        n.ActionType == 50 ? "Monitoring" :
                                                        n.ActionType == 60 ? "Administrative" : null,
                                         AssignedTeam = db.MaintenanceTeams.Where(u => u.Id == n.MaintenanceStaffId).Select(n => n.Name).FirstOrDefault(),
                                         ContactNumber = db.MaintenanceTeams.Where(u => u.Id == n.MaintenanceStaffId).Select(n => n.ContactNumber).FirstOrDefault(),
                                         Priority = n.Priority == 10 ? "Low" :
                                                      n.Priority == 20 ? "Moderate" :
                                                      n.Priority == 30 ? "High" : ""

                                     }).OrderBy(n => n.DueDate).AsNoTracking().ToListAsync();


                // Load the row values on the Main UI Thread
                this.BeginInvoke(() =>
                {
                    dataGridView2.DataSource = null;
                    dataGridView2.Rows.Clear();

                    foreach (var record in actionPlanList)
                    {
                        int rowIndex = dataGridView2.Rows.Add(
                             record.Id,
                             record.ActionDetail,
                             record.IncidentTitle,
                             record.DueDate.ToString("yyyy-MM-dd"),
                             record.ActionType,
                             record.AssignedTeam,
                             record.ContactNumber,
                             record.Priority
                         );

                        var row = dataGridView2.Rows[rowIndex];

                        if (record.Priority == "Low")
                        {
                            row.DefaultCellStyle.ForeColor = Color.White;
                            row.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                        }
                        else if (record.Priority == "Moderate")
                        {
                            row.DefaultCellStyle.BackColor = Color.Yellow;
                            row.DefaultCellStyle.ForeColor = Color.Black;
                            row.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                        }
                        else if (record.Priority == "High")
                        {
                            row.DefaultCellStyle.BackColor = Color.Red;
                            row.DefaultCellStyle.ForeColor = Color.Black;
                            row.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                        }
                    }
                });

            }
        }

        private async Task ActionPlanMonitoringSafe()
        {
            if (_actionPlanMonitoringSafe) return;
            _actionPlanMonitoringSafe = false;


            try
            {
                await ActionPlanMonitoring();
            }
            finally
            {
                _actionPlanMonitoringSafe = false;
            }
        }

        private async Task OfficerResponseMonitoring()
        {
            using (var db = new RenjiDbContext())
            {
                var todaysDate = DateTime.UtcNow;

                var idleUserLists = await (from ap in db.ActionPlans
                                           join mt in db.MaintenanceTeams
                                           on ap.MaintenanceStaffId equals mt.Id
                                           where ap.DueDate < DateTime.UtcNow && ap.Status != 30
                                           group ap by mt.Name into g
                                           select new
                                           {
                                               Name = g.Key,
                                               NumberOfDuePlans = g.Count()
                                           }
                                     ).AsNoTracking().ToListAsync();

                var maintenanceTeamRecord = await db.MaintenanceTeams.Select(n => new
                {
                    Team = n.Name,
                    ContactNumber = n.ContactNumber,
                    Email = n.Email,

                }).ToListAsync();


                // Load the row values to the Main UI Thread
                this.BeginInvoke(() =>
                {
                    dataGridView3.DataSource = null;
                    dataGridView3.Rows.Clear();

                    dataGridView4.DataSource = null;
                    dataGridView4.Rows.Clear();

                    foreach (var record in idleUserLists)
                    {
                        dataGridView3.Rows.Add(
                             record.Name,
                             record.NumberOfDuePlans
                        );
                    }

                    foreach (var record in maintenanceTeamRecord)
                    {
                        dataGridView4.Rows.Add(
                             record.Team,
                             record.ContactNumber,
                             record.Email
                        );
                    }


                });
            }
        }

        private async Task OfficerResponseMonitoringSafe()
        {
            if (_officerResponseMonitoringSafe) return;
            _officerResponseMonitoringSafe = false;


            try
            {
                await OfficerResponseMonitoring();
            }
            finally
            {
                _officerResponseMonitoringSafe = false;
            }
        }

        // Check Action Plans that is already in Due Date and send email notification to the assigning maintenance staff
        private async Task CheckDueActionPlan()
        {
            DateTime dateToday = DateTime.UtcNow;

            // Get all action plan that is due already
            var actionPlanRecord = await db.ActionPlans.Where(u => u.DueDate < dateToday).ToListAsync();


            foreach (var actionPlan in actionPlanRecord)
            {
                if (actionPlan.IsOverDueNotified == false)
                {
                    string emailAddress = await db.MaintenanceTeams.Where(u => u.Id == actionPlan.MaintenanceStaffId).Select(n => n.Email).FirstOrDefaultAsync() ?? "";

                    var result = await emailNotification.SendEmail(actionPlan.Id, emailAddress);

                    if (result.isSuccess)
                    {
                        actionPlan.IsOverDueNotified = true;
                        await db.SaveChangesAsync();
                    }

                }
            }

        }


        private async Task CheckDueActionPlanSafe()
        {
            if (_checkDueActionPlanSafe) return;
            _checkDueActionPlanSafe = false;


            try
            {
                await CheckDueActionPlan();
            }
            finally
            {
                _checkDueActionPlanSafe = false;
            }
        }

        private async Task DueActionPlanMonitoring()
        {
            using (var db = new RenjiDbContext())
            {
                var todaysDate = DateTime.UtcNow;

                var actionPlanList = await db.ActionPlans.
                                     Where(u => u.Status != 30). // Filter action plans that are not yet completed
                                     Where(u => u.DueDate < todaysDate).
                                     Select(n => new
                                     {   
                                         Id = n.Id,
                                         ActionDetail = n.ActionDetail,
                                         IncidentTitle = db.IncidentReports.Where(u => u.Id == n.IncidentReportId).Select(n => n.Title).FirstOrDefault(),
                                         DueDate = n.DueDate.Date,
                                         ActionType = n.ActionType == 10 ? "Corrective" :
                                                        n.ActionType == 20 ? "Preventive" :
                                                        n.ActionType == 30 ? "Mitigation" :
                                                        n.ActionType == 40 ? "Containment" :
                                                        n.ActionType == 50 ? "Monitoring" :
                                                        n.ActionType == 60 ? "Administrative" : null,
                                         AssignedTeam = db.MaintenanceTeams.Where(u => u.Id == n.MaintenanceStaffId).Select(n => n.Name).FirstOrDefault(),
                                         ContactNumber = db.MaintenanceTeams.Where(u => u.Id == n.MaintenanceStaffId).Select(n => n.ContactNumber).FirstOrDefault(),
                                         Priority = n.Priority == 10 ? "Low" :
                                                      n.Priority == 20 ? "Moderate" :
                                                      n.Priority == 30 ? "High" : ""

                                     }).OrderBy(n => n.DueDate).AsNoTracking().ToListAsync();


                // Load the row values on the Main UI Thread
                this.BeginInvoke(() =>
                {
                    dataGridView5.DataSource = null;
                    dataGridView5.Rows.Clear();

                    foreach (var record in actionPlanList)
                    {
                        dataGridView5.Rows.Add(
                             record.Id,
                             record.ActionDetail,
                             record.IncidentTitle,
                             record.DueDate.ToString("yyyy-MM-dd"),
                             record.ActionType,
                             record.AssignedTeam,
                             record.ContactNumber,
                             record.Priority
                         );

                    }
                });

            }
        }

        private async Task DueActionPlanMonitoringSafe()
        {
            if (_dueActionPlanMonitoringSafe) return;
            _dueActionPlanMonitoringSafe = false;


            try
            {
                await DueActionPlanMonitoring();
            }
            finally
            {
                _dueActionPlanMonitoringSafe = false;
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
