using DevExpress.Utils.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Winform4System.Helpers
{
    public static class SvgIconCatalog
    {
        private static readonly string StartupPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string IconsPath = ResolveIconsPath();

        public static readonly SvgImage SelectionChecked = Load("selection-checked.svg");
        public static readonly SvgImage SelectionUnchecked = Load("selection-unchecked.svg");
        public static readonly SvgImage Add = Load("action-add.svg");
        public static readonly SvgImage Attachment = Load("action-attachment.svg");
        public static readonly SvgImage Cancel = Load("action-cancel.svg");
        public static readonly SvgImage Close = Load("action-close.svg");
        public static readonly SvgImage Confirm = Load("action-confirm.svg");
        public static readonly SvgImage Copy = Load("action-copy.svg");
        public static readonly SvgImage Delete = Load("action-delete.svg");
        public static readonly SvgImage Edit = Load("action-edit.svg");
        public static readonly SvgImage Filter = Load("action-filter.svg");
        public static readonly SvgImage Info = Load("action-info.svg");
        public static readonly SvgImage Learn = Load("action-learn.svg");
        public static readonly SvgImage Plus = Load("action-plus.svg");
        public static readonly SvgImage Print = Load("action-print.svg");
        public static readonly SvgImage Refresh = Load("action-refresh.svg");
        public static readonly SvgImage Search = Load("action-search.svg");
        public static readonly SvgImage Signature = Load("action-signature.svg");
        public static readonly SvgImage Transfer = Load("action-transfer.svg");
        public static readonly SvgImage Upload = Load("action-upload.svg");
        public static readonly SvgImage View = Load("action-view.svg");

        public static readonly SvgImage ApprovalStamp = Load("approval-stamp.svg");
        public static readonly SvgImage Bot = Load("automation-bot.svg");
        public static readonly SvgImage Robot = Load("automation-robot.svg");
        public static readonly SvgImage CalendarAdd = Load("calendar-add.svg");
        public static readonly SvgImage Schedule = Load("calendar-schedule.svg");
        public static readonly SvgImage SendEmail = Load("communication-email-send.svg");
        public static readonly SvgImage ExportExcel = Load("file-excel.svg");
        public static readonly SvgImage GoogleForms = Load("file-google-forms.svg");
        public static readonly SvgImage GoogleSheets = Load("file-google-sheets.svg");
        public static readonly SvgImage Word = Load("file-word.svg");
        public static readonly SvgImage Cost = Load("finance-cost.svg");
        public static readonly SvgImage Promote = Load("hierarchy-promote.svg");
        public static readonly SvgImage GasStation = Load("location-gas-station.svg");
        public static readonly SvgImage Equipment = Load("navigation-equipment.svg");
        public static readonly SvgImage Department = Load("organization-department.svg");
        public static readonly SvgImage Progress = Load("process-progress.svg");
        public static readonly SvgImage Denied = Load("status-denied.svg");
        public static readonly SvgImage Disabled = Load("status-disabled.svg");
        public static readonly SvgImage Finished = Load("status-finished.svg");
        public static readonly SvgImage StartTime = Load("time-start.svg");
        public static readonly SvgImage ChangeUser = Load("user-change.svg");
        public static readonly SvgImage AddUserGroup = Load("user-group-add.svg");
        public static readonly SvgImage SuspendUser = Load("user-suspend.svg");
        public static readonly SvgImage UserTransfer = Load("user-transfer.svg");

        public static readonly SvgImage Step1 = Load("step-1.svg");
        public static readonly SvgImage Step2 = Load("step-2.svg");
        public static readonly SvgImage Step3 = Load("step-3.svg");
        public static readonly SvgImage Step4 = Load("step-4.svg");
        public static readonly SvgImage Step5 = Load("step-5.svg");
        public static readonly SvgImage Step6 = Load("step-6.svg");

        private static string ResolveIconsPath()
        {
            foreach (string root in GetCandidateRoots())
            {
                foreach (string candidate in GetCandidateIconFolders(root))
                {
                    if (Directory.Exists(candidate) && File.Exists(Path.Combine(candidate, "action-add.svg")))
                    {
                        return candidate;
                    }
                }
            }

            return Path.Combine(StartupPath, "Images");
        }

        private static IEnumerable<string> GetCandidateRoots()
        {
            var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            AddRoot(roots, AppDomain.CurrentDomain.BaseDirectory);
            AddRoot(roots, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            AddRoot(roots, Environment.CurrentDirectory);
            return roots;
        }

        private static void AddRoot(HashSet<string> roots, string root)
        {
            if (!string.IsNullOrWhiteSpace(root) && Directory.Exists(root))
            {
                roots.Add(root);
            }
        }

        private static IEnumerable<string> GetCandidateIconFolders(string root)
        {
            string current = root;
            while (!string.IsNullOrWhiteSpace(current) && Directory.Exists(current))
            {
                yield return Path.Combine(current, "Images");
                yield return Path.Combine(current, "Winform4System", "Images");

                DirectoryInfo parent = Directory.GetParent(current);
                current = parent?.FullName;
            }
        }

        private static SvgImage Load(string fileName)
        {
            string filePath = Path.Combine(IconsPath, fileName);
            try
            {
                return File.Exists(filePath) ? SvgImage.FromFile(filePath) : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
