namespace BTLtruc_quan.WinForms.Forms;

partial class ComponentGalleryForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        // 
        // ComponentGalleryForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.ClientSize = new System.Drawing.Size(1200, 800);
        this.MinimumSize = new System.Drawing.Size(1024, 680);
        this.Name = "ComponentGalleryForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "SportChain VN - UI Component Gallery & DPI Verification";
        this.ResumeLayout(false);
    }
}
