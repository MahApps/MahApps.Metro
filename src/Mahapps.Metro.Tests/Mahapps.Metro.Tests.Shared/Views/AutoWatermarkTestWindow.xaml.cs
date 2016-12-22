namespace MahApps.Metro.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    public partial class AutoWatermarkTestWindow
    {
        public AutoWatermarkTestWindow()
        {
            InitializeComponent();
        }
    }

    public class AutoWatermarkTestModel
    {
        [Display(Prompt = "AutoWatermark")]
        public string TextBoxText { get; set; }

        [Display(Prompt = "AutoWatermark")]
        public object ComboBoxSelectedObject { get; set; }

        [Display(Prompt = "AutoWatermark")]
        public double? NumericUpDownValue { get; set; }

        [Display(Prompt = "AutoWatermark")]
        public DateTime? DatePickerDate { get; set; }

        public AutoWatermarkTestSubModel SubModel { get; set; }= new AutoWatermarkTestSubModel();

        public ObservableCollection<AutoWatermarkTestSubModel> CollectionProperty { get; set; } = new ObservableCollection<AutoWatermarkTestSubModel>(new [] { new AutoWatermarkTestSubModel() });
    }

    public class AutoWatermarkTestSubModel
    {
        [Display(Prompt = "AutoWatermark")]
        public string TextBoxText { get; set; }
    }
}