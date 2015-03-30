using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
namespace CertLister
{
	public class CertLister : Form
	{
		private IContainer components = null;
		private TreeView tv;
		private TextBox tbxout;
		public CertLister()
		{
			this.InitializeComponent();
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			this.tv.AfterSelect += new TreeViewEventHandler(this.tv_AfterSelect);
			this.BuildNode(StoreName.AddressBook);
			this.BuildNode(StoreName.AuthRoot);
			this.BuildNode(StoreName.CertificateAuthority);
			this.BuildNode(StoreName.Disallowed);
			this.BuildNode(StoreName.My);
			this.BuildNode(StoreName.Root);
			this.BuildNode(StoreName.TrustedPeople);
			this.BuildNode(StoreName.TrustedPublisher);
		}
		private void tv_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Parent != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StoreName storeName;
				if (!Enum.TryParse<StoreName>(e.Node.Parent.Text, out storeName))
				{
					throw new ApplicationException("Could not parse storename");
				}
				StoreLocation storeLocation;
				if (!Enum.TryParse<StoreLocation>(e.Node.Text, out storeLocation))
				{
					throw new ApplicationException("Could not parse store location");
				}
				X509Store x509Store = new X509Store(storeName, storeLocation);
				x509Store.Open(OpenFlags.ReadOnly);
				X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
				while (enumerator.MoveNext())
				{
					X509Certificate2 current = enumerator.Current;
					string value = "No friendly name given";
					if (!string.IsNullOrWhiteSpace(current.FriendlyName))
					{
						value = current.FriendlyName;
					}
					stringBuilder.Append(value);
					stringBuilder.Append(", ");
					stringBuilder.Append(current.SubjectName.Name);
					stringBuilder.Append(" (");
					stringBuilder.Append(current.Thumbprint);
					stringBuilder.AppendLine(")");
					stringBuilder.AppendLine("");
				}
				this.tbxout.Text = stringBuilder.ToString();
			}
			else
			{
				this.tbxout.Text = "";
			}
		}
		private void BuildNode(StoreName name)
		{
			TreeNode[] children = new TreeNode[]
			{
				this.GetLocation(name, StoreLocation.CurrentUser),
				this.GetLocation(name, StoreLocation.LocalMachine)
			};
			TreeNode node = new TreeNode(name.ToString(), children);
			this.tv.Nodes.Add(node);
		}
		private TreeNode GetLocation(StoreName name, StoreLocation location)
		{
			return new TreeNode(location.ToString());
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			this.tv = new TreeView();
			this.tbxout = new TextBox();
			base.SuspendLayout();
			this.tv.Dock = DockStyle.Left;
			this.tv.Location = new Point(0, 0);
			this.tv.Name = "tv";
			this.tv.Size = new Size(181, 629);
			this.tv.TabIndex = 0;
			this.tbxout.Dock = DockStyle.Fill;
			this.tbxout.Location = new Point(181, 0);
			this.tbxout.Multiline = true;
			this.tbxout.Name = "tbxout";
			this.tbxout.Size = new Size(712, 629);
			this.tbxout.TabIndex = 1;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(893, 629);
			base.Controls.Add(this.tbxout);
			base.Controls.Add(this.tv);
			base.Name = "CertLister";
			this.Text = "CertLister";
			base.Load += new EventHandler(this.Form1_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
