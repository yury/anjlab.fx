using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace AnjLab.FX.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for SelectFontDialog.xaml
    /// </summary>
    public partial class SelectFontDialog
    {
        #region Private fields and types

        private static readonly double[] CommonlyUsedFontSizes = new[]
                                                                     {
                                                                         3.0, 4.0, 5.0, 6.0, 6.5,
                                                                         7.0, 7.5, 8.0, 8.5, 9.0,
                                                                         9.5, 10.0, 10.5, 11.0, 11.5,
                                                                         12.0, 12.5, 13.0, 13.5, 14.0,
                                                                         15.0, 16.0, 17.0, 18.0, 19.0,
                                                                         20.0, 22.0, 24.0, 26.0, 28.0, 30.0, 32.0, 34.0,
                                                                         36.0, 38.0,
                                                                         40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0,
                                                                         72.0, 76.0,
                                                                         80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0,
                                                                         136.0, 144.0
                                                                     };


        private string _defaultSampleText;
        private ICollection<FontFamily> _familyCollection; // see FamilyCollection property
        private bool _familyListValid; // indicates the list of font families is valid
        private string _previewSampleText;
        private bool _previewValid; // indicates the preview control is valid
        private bool _typefaceListSelectionValid; // indicates the current selection in the typeface list is valid
        private bool _typefaceListValid; // indicates the list of typefaces is valid
        private bool _updatePending; // indicates a call to OnUpdate is scheduled

        // Specialized metadata object for font chooser dependency properties

        #region Nested type: FontPropertyMetadata

        private class FontPropertyMetadata : FrameworkPropertyMetadata
        {
            public readonly DependencyProperty TargetProperty;

            public FontPropertyMetadata(
                object defaultValue,
                PropertyChangedCallback changeCallback,
                DependencyProperty targetProperty
                )
                : base(defaultValue, changeCallback)
            {
                TargetProperty = targetProperty;
            }
        }

        #endregion

        // Specialized metadata object for typographic font chooser properties

        #region Nested type: TypographicPropertyMetadata

        private class TypographicPropertyMetadata : FontPropertyMetadata
        {
            private static readonly PropertyChangedCallback _callback = TypographicPropertyChangedCallback;
            public readonly TypographyFeaturePage FeaturePage;
            public readonly string SampleTextTag;

            public TypographicPropertyMetadata(object defaultValue, DependencyProperty targetProperty,
                                               TypographyFeaturePage featurePage, string sampleTextTag)
                : base(defaultValue, _callback, targetProperty)
            {
                FeaturePage = featurePage;
                SampleTextTag = sampleTextTag;
            }
        }

        #endregion

        // Object used to initialize the right-hand side of the typographic properties tab

        #region Nested type: TypographyFeaturePage

        private class TypographyFeaturePage
        {
            public static readonly TypographyFeaturePage BooleanFeaturePage = new TypographyFeaturePage(
                new[]
                    {
                        new Item("Disabled", false),
                        new Item("Enabled", true)
                    }
                );

            public static readonly TypographyFeaturePage IntegerFeaturePage = new TypographyFeaturePage(
                new[]
                    {
                        new Item("_0", 0),
                        new Item("_1", 1),
                        new Item("_2", 2),
                        new Item("_3", 3),
                        new Item("_4", 4),
                        new Item("_5", 5),
                        new Item("_6", 6),
                        new Item("_7", 7),
                        new Item("_8", 8),
                        new Item("_9", 9)
                    }
                );

            public readonly Item[] Items;

            public TypographyFeaturePage(Item[] items)
            {
                Items = items;
            }

            public TypographyFeaturePage(Type enumType)
            {
                string[] names = Enum.GetNames(enumType);
                Array values = Enum.GetValues(enumType);

                Items = new Item[names.Length];

                for (int i = 0; i < names.Length; ++i)
                {
                    Items[i] = new Item(names[i], values.GetValue(i));
                }
            }

            #region Nested type: Item

            public struct Item
            {
                public readonly string Tag;
                public readonly object Value;

                public Item(string tag, object value)
                {
                    Tag = tag;
                    Value = value;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: UpdateCallback

        private delegate void UpdateCallback();

        #endregion

        // Encapsulates the state and initialization logic of a tab control item.

        #endregion

        #region Construction and initialization

        public SelectFontDialog()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _previewSampleText = _defaultSampleText = previewTextBox.Text;

            // Hook up events for the font family list and associated text box.
            fontFamilyTextBox.SelectionChanged += fontFamilyTextBox_SelectionChanged;
            fontFamilyTextBox.TextChanged += fontFamilyTextBox_TextChanged;
            fontFamilyTextBox.PreviewKeyDown += fontFamilyTextBox_PreviewKeyDown;
            fontFamilyList.SelectionChanged += fontFamilyList_SelectionChanged;

            // Hook up events for the typeface list.
            typefaceList.SelectionChanged += typefaceList_SelectionChanged;

            // Hook up events for the font size list and associated text box.
            sizeTextBox.TextChanged += sizeTextBox_TextChanged;
            sizeTextBox.PreviewKeyDown += sizeTextBox_PreviewKeyDown;
            sizeList.SelectionChanged += sizeList_SelectionChanged;

            // Hook up events for text decoration check boxes.
            RoutedEventHandler textDecorationEventHandler = textDecorationCheckStateChanged;
            underlineCheckBox.Checked += textDecorationEventHandler;
            underlineCheckBox.Unchecked += textDecorationEventHandler;
            baselineCheckBox.Checked += textDecorationEventHandler;
            baselineCheckBox.Unchecked += textDecorationEventHandler;
            strikethroughCheckBox.Checked += textDecorationEventHandler;
            strikethroughCheckBox.Unchecked += textDecorationEventHandler;
            overlineCheckBox.Checked += textDecorationEventHandler;
            overlineCheckBox.Unchecked += textDecorationEventHandler;

            // Initialize the list of font sizes and select the nearest size.
            foreach (double value in CommonlyUsedFontSizes)
            {
                sizeList.Items.Add(new FontSizeListItem(value));
            }
            OnSelectedFontSizeChanged(SelectedFontSize);

            // Initialize the font family list and the current family.
            if (!_familyListValid)
            {
                InitializeFontFamilyList();
                _familyListValid = true;
                OnSelectedFontFamilyChanged(SelectedFontFamily);
            }

            // Schedule background updates.
            ScheduleUpdate();
        }

        #endregion

        #region Event handlers

        private int _fontFamilyTextBoxSelectionStart;

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void fontFamilyTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _fontFamilyTextBoxSelectionStart = fontFamilyTextBox.SelectionStart;
        }

        private void fontFamilyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = fontFamilyTextBox.Text;

            // Update the current list item.
            if (SelectFontFamilyListItem(text) == null)
            {
                // The text does not exactly match a family name so consider applying auto-complete behavior.
                // However, only do so if the following conditions are met:
                //   (1)  The user is typing more text rather than deleting (i.e., the new text length is
                //        greater than the most recent selection start index), and
                //   (2)  The caret is at the end of the text box.
                if (text.Length > _fontFamilyTextBoxSelectionStart
                    && fontFamilyTextBox.SelectionStart == text.Length)
                {
                    // Get the current list item, which should be the nearest match for the text.
                    var item = fontFamilyList.Items.CurrentItem as FontFamilyListItem;
                    if (item != null)
                    {
                        // Does the text box text match the beginning of the family name?
                        string familyDisplayName = item.ToString();
                        if (
                            string.Compare(text, 0, familyDisplayName, 0, text.Length, true, CultureInfo.CurrentCulture) ==
                            0)
                        {
                            // Set the text box text to the complete family name and select the part not typed in.
                            fontFamilyTextBox.Text = familyDisplayName;
                            fontFamilyTextBox.SelectionStart = text.Length;
                            fontFamilyTextBox.SelectionLength = familyDisplayName.Length - text.Length;
                        }
                    }
                }
            }
        }

        private void sizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double sizeInPoints;
            if (double.TryParse(sizeTextBox.Text, out sizeInPoints))
            {
                double sizeInPixels = FontSizeListItem.PointsToPixels(sizeInPoints);
                if (!FontSizeListItem.FuzzyEqual(sizeInPixels, SelectedFontSize))
                {
                    SelectedFontSize = sizeInPixels;
                }
            }
        }

        private void fontFamilyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnComboBoxPreviewKeyDown(fontFamilyTextBox, fontFamilyList, e);
        }

        private void sizeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnComboBoxPreviewKeyDown(sizeTextBox, sizeList, e);
        }

        private void fontFamilyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = fontFamilyList.SelectedItem as FontFamilyListItem;
            if (item != null)
            {
                SelectedFontFamily = item.FontFamily;
            }
        }

        private void sizeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = sizeList.SelectedItem as FontSizeListItem;
            if (item != null)
            {
                SelectedFontSize = item.SizeInPixels;
            }
        }

        private void typefaceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = typefaceList.SelectedItem as TypefaceListItem;
            if (item != null)
            {
                SelectedFontWeight = item.FontWeight;
                SelectedFontStyle = item.FontStyle;
                SelectedFontStretch = item.FontStretch;
            }
        }

        private void textDecorationCheckStateChanged(object sender, RoutedEventArgs e)
        {
            var textDecorations = new TextDecorationCollection();

            if (underlineCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.Underline[0]);
            }
            if (baselineCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.Baseline[0]);
            }
            if (strikethroughCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.Strikethrough[0]);
            }
            if (overlineCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.OverLine[0]);
            }

            textDecorations.Freeze();
            SelectedTextDecorations = textDecorations;
        }

        #endregion

        #region Public properties and methods

        /// <summary>
        /// Collection of font families to display in the font family list. By default this is Fonts.SystemFontFamilies,
        /// but a client could set this to another collection returned by Fonts.GetFontFamilies, e.g., a collection of
        /// application-defined fonts.
        /// </summary>
        public ICollection<FontFamily> FontFamilyCollection
        {
            get { return (_familyCollection == null) ? Fonts.SystemFontFamilies : _familyCollection; }

            set
            {
                if (value != _familyCollection)
                {
                    _familyCollection = value;
                    InvalidateFontFamilyList();
                }
            }
        }

        /// <summary>
        /// Sample text used in the preview box and family and typeface samples tab.
        /// </summary>
        public string PreviewSampleText
        {
            get { return _previewSampleText; }

            set
            {
                string newValue = string.IsNullOrEmpty(value) ? _defaultSampleText : value;
                if (newValue != _previewSampleText)
                {
                    _previewSampleText = newValue;

                    // Update the preview text box.
                    previewTextBox.Text = newValue;
                }
            }
        }

        /// <summary>
        /// Sets the font chooser selection properties to match the properites of the specified object.
        /// </summary>
        public void SetPropertiesFromObject(DependencyObject obj)
        {
            foreach (DependencyProperty property in _chooserProperties)
            {
                var metadata = property.GetMetadata(typeof(SelectFontDialog)) as FontPropertyMetadata;
                if (metadata != null)
                {
                    SetValue(property, obj.GetValue(metadata.TargetProperty));
                }
            }
        }

        /// <summary>
        /// Sets the properites of the specified object to match the font chooser selection properties.
        /// </summary>
        public void ApplyPropertiesToObject(DependencyObject obj)
        {
            foreach (DependencyProperty property in _chooserProperties)
            {
                var metadata = property.GetMetadata(typeof(SelectFontDialog)) as FontPropertyMetadata;
                if (metadata != null)
                {
                    obj.SetValue(metadata.TargetProperty, GetValue(property));
                }
            }
        }

        #endregion

        #region Dependency properties for typographic features

        public static readonly DependencyProperty AnnotationAlternatesProperty =
            RegisterTypographicProperty(Typography.AnnotationAlternatesProperty);

        public static readonly DependencyProperty CapitalSpacingProperty =
            RegisterTypographicProperty(Typography.CapitalSpacingProperty);

        public static readonly DependencyProperty CapitalsProperty =
            RegisterTypographicProperty(Typography.CapitalsProperty);

        public static readonly DependencyProperty CaseSensitiveFormsProperty =
            RegisterTypographicProperty(Typography.CaseSensitiveFormsProperty);

        public static readonly DependencyProperty ContextualAlternatesProperty =
            RegisterTypographicProperty(Typography.ContextualAlternatesProperty);

        public static readonly DependencyProperty ContextualLigaturesProperty =
            RegisterTypographicProperty(Typography.ContextualLigaturesProperty);

        public static readonly DependencyProperty ContextualSwashesProperty =
            RegisterTypographicProperty(Typography.ContextualSwashesProperty);

        public static readonly DependencyProperty DiscretionaryLigaturesProperty =
            RegisterTypographicProperty(Typography.DiscretionaryLigaturesProperty);

        public static readonly DependencyProperty EastAsianExpertFormsProperty =
            RegisterTypographicProperty(Typography.EastAsianExpertFormsProperty);

        public static readonly DependencyProperty EastAsianLanguageProperty =
            RegisterTypographicProperty(Typography.EastAsianLanguageProperty);

        public static readonly DependencyProperty EastAsianWidthsProperty =
            RegisterTypographicProperty(Typography.EastAsianWidthsProperty);

        public static readonly DependencyProperty FractionProperty =
            RegisterTypographicProperty(Typography.FractionProperty, "OneHalf");

        public static readonly DependencyProperty HistoricalFormsProperty =
            RegisterTypographicProperty(Typography.HistoricalFormsProperty);

        public static readonly DependencyProperty HistoricalLigaturesProperty =
            RegisterTypographicProperty(Typography.HistoricalLigaturesProperty);

        public static readonly DependencyProperty KerningProperty =
            RegisterTypographicProperty(Typography.KerningProperty);

        public static readonly DependencyProperty MathematicalGreekProperty =
            RegisterTypographicProperty(Typography.MathematicalGreekProperty);

        public static readonly DependencyProperty NumeralAlignmentProperty =
            RegisterTypographicProperty(Typography.NumeralAlignmentProperty, "Digits");

        public static readonly DependencyProperty NumeralStyleProperty =
            RegisterTypographicProperty(Typography.NumeralStyleProperty, "Digits");

        public static readonly DependencyProperty SlashedZeroProperty =
            RegisterTypographicProperty(Typography.SlashedZeroProperty, "Digits");

        public static readonly DependencyProperty StandardLigaturesProperty =
            RegisterTypographicProperty(Typography.StandardLigaturesProperty);

        public static readonly DependencyProperty StandardSwashesProperty =
            RegisterTypographicProperty(Typography.StandardSwashesProperty);

        public static readonly DependencyProperty StylisticAlternatesProperty =
            RegisterTypographicProperty(Typography.StylisticAlternatesProperty);

        public static readonly DependencyProperty StylisticSet10Property =
            RegisterTypographicProperty(Typography.StylisticSet10Property);

        public static readonly DependencyProperty StylisticSet11Property =
            RegisterTypographicProperty(Typography.StylisticSet11Property);

        public static readonly DependencyProperty StylisticSet12Property =
            RegisterTypographicProperty(Typography.StylisticSet12Property);

        public static readonly DependencyProperty StylisticSet13Property =
            RegisterTypographicProperty(Typography.StylisticSet13Property);

        public static readonly DependencyProperty StylisticSet14Property =
            RegisterTypographicProperty(Typography.StylisticSet14Property);

        public static readonly DependencyProperty StylisticSet15Property =
            RegisterTypographicProperty(Typography.StylisticSet15Property);

        public static readonly DependencyProperty StylisticSet16Property =
            RegisterTypographicProperty(Typography.StylisticSet16Property);

        public static readonly DependencyProperty StylisticSet17Property =
            RegisterTypographicProperty(Typography.StylisticSet17Property);

        public static readonly DependencyProperty StylisticSet18Property =
            RegisterTypographicProperty(Typography.StylisticSet18Property);

        public static readonly DependencyProperty StylisticSet19Property =
            RegisterTypographicProperty(Typography.StylisticSet19Property);

        public static readonly DependencyProperty StylisticSet1Property =
            RegisterTypographicProperty(Typography.StylisticSet1Property);

        public static readonly DependencyProperty StylisticSet20Property =
            RegisterTypographicProperty(Typography.StylisticSet20Property);

        public static readonly DependencyProperty StylisticSet2Property =
            RegisterTypographicProperty(Typography.StylisticSet2Property);

        public static readonly DependencyProperty StylisticSet3Property =
            RegisterTypographicProperty(Typography.StylisticSet3Property);

        public static readonly DependencyProperty StylisticSet4Property =
            RegisterTypographicProperty(Typography.StylisticSet4Property);

        public static readonly DependencyProperty StylisticSet5Property =
            RegisterTypographicProperty(Typography.StylisticSet5Property);

        public static readonly DependencyProperty StylisticSet6Property =
            RegisterTypographicProperty(Typography.StylisticSet6Property);

        public static readonly DependencyProperty StylisticSet7Property =
            RegisterTypographicProperty(Typography.StylisticSet7Property);

        public static readonly DependencyProperty StylisticSet8Property =
            RegisterTypographicProperty(Typography.StylisticSet8Property);

        public static readonly DependencyProperty StylisticSet9Property =
            RegisterTypographicProperty(Typography.StylisticSet9Property);

        public static readonly DependencyProperty VariantsProperty =
            RegisterTypographicProperty(Typography.VariantsProperty);

        public bool StandardLigatures
        {
            get { return (bool) GetValue(StandardLigaturesProperty); }
            set { SetValue(StandardLigaturesProperty, value); }
        }

        public bool ContextualLigatures
        {
            get { return (bool) GetValue(ContextualLigaturesProperty); }
            set { SetValue(ContextualLigaturesProperty, value); }
        }

        public bool DiscretionaryLigatures
        {
            get { return (bool) GetValue(DiscretionaryLigaturesProperty); }
            set { SetValue(DiscretionaryLigaturesProperty, value); }
        }

        public bool HistoricalLigatures
        {
            get { return (bool) GetValue(HistoricalLigaturesProperty); }
            set { SetValue(HistoricalLigaturesProperty, value); }
        }

        public bool ContextualAlternates
        {
            get { return (bool) GetValue(ContextualAlternatesProperty); }
            set { SetValue(ContextualAlternatesProperty, value); }
        }

        public bool HistoricalForms
        {
            get { return (bool) GetValue(HistoricalFormsProperty); }
            set { SetValue(HistoricalFormsProperty, value); }
        }

        public bool Kerning
        {
            get { return (bool) GetValue(KerningProperty); }
            set { SetValue(KerningProperty, value); }
        }

        public bool CapitalSpacing
        {
            get { return (bool) GetValue(CapitalSpacingProperty); }
            set { SetValue(CapitalSpacingProperty, value); }
        }

        public bool CaseSensitiveForms
        {
            get { return (bool) GetValue(CaseSensitiveFormsProperty); }
            set { SetValue(CaseSensitiveFormsProperty, value); }
        }

        public bool StylisticSet1
        {
            get { return (bool) GetValue(StylisticSet1Property); }
            set { SetValue(StylisticSet1Property, value); }
        }

        public bool StylisticSet2
        {
            get { return (bool) GetValue(StylisticSet2Property); }
            set { SetValue(StylisticSet2Property, value); }
        }

        public bool StylisticSet3
        {
            get { return (bool) GetValue(StylisticSet3Property); }
            set { SetValue(StylisticSet3Property, value); }
        }

        public bool StylisticSet4
        {
            get { return (bool) GetValue(StylisticSet4Property); }
            set { SetValue(StylisticSet4Property, value); }
        }

        public bool StylisticSet5
        {
            get { return (bool) GetValue(StylisticSet5Property); }
            set { SetValue(StylisticSet5Property, value); }
        }

        public bool StylisticSet6
        {
            get { return (bool) GetValue(StylisticSet6Property); }
            set { SetValue(StylisticSet6Property, value); }
        }

        public bool StylisticSet7
        {
            get { return (bool) GetValue(StylisticSet7Property); }
            set { SetValue(StylisticSet7Property, value); }
        }

        public bool StylisticSet8
        {
            get { return (bool) GetValue(StylisticSet8Property); }
            set { SetValue(StylisticSet8Property, value); }
        }

        public bool StylisticSet9
        {
            get { return (bool) GetValue(StylisticSet9Property); }
            set { SetValue(StylisticSet9Property, value); }
        }

        public bool StylisticSet10
        {
            get { return (bool) GetValue(StylisticSet10Property); }
            set { SetValue(StylisticSet10Property, value); }
        }

        public bool StylisticSet11
        {
            get { return (bool) GetValue(StylisticSet11Property); }
            set { SetValue(StylisticSet11Property, value); }
        }

        public bool StylisticSet12
        {
            get { return (bool) GetValue(StylisticSet12Property); }
            set { SetValue(StylisticSet12Property, value); }
        }

        public bool StylisticSet13
        {
            get { return (bool) GetValue(StylisticSet13Property); }
            set { SetValue(StylisticSet13Property, value); }
        }

        public bool StylisticSet14
        {
            get { return (bool) GetValue(StylisticSet14Property); }
            set { SetValue(StylisticSet14Property, value); }
        }

        public bool StylisticSet15
        {
            get { return (bool) GetValue(StylisticSet15Property); }
            set { SetValue(StylisticSet15Property, value); }
        }

        public bool StylisticSet16
        {
            get { return (bool) GetValue(StylisticSet16Property); }
            set { SetValue(StylisticSet16Property, value); }
        }

        public bool StylisticSet17
        {
            get { return (bool) GetValue(StylisticSet17Property); }
            set { SetValue(StylisticSet17Property, value); }
        }

        public bool StylisticSet18
        {
            get { return (bool) GetValue(StylisticSet18Property); }
            set { SetValue(StylisticSet18Property, value); }
        }

        public bool StylisticSet19
        {
            get { return (bool) GetValue(StylisticSet19Property); }
            set { SetValue(StylisticSet19Property, value); }
        }

        public bool StylisticSet20
        {
            get { return (bool) GetValue(StylisticSet20Property); }
            set { SetValue(StylisticSet20Property, value); }
        }

        public bool SlashedZero
        {
            get { return (bool) GetValue(SlashedZeroProperty); }
            set { SetValue(SlashedZeroProperty, value); }
        }

        public bool MathematicalGreek
        {
            get { return (bool) GetValue(MathematicalGreekProperty); }
            set { SetValue(MathematicalGreekProperty, value); }
        }

        public bool EastAsianExpertForms
        {
            get { return (bool) GetValue(EastAsianExpertFormsProperty); }
            set { SetValue(EastAsianExpertFormsProperty, value); }
        }

        public FontFraction Fraction
        {
            get { return (FontFraction) GetValue(FractionProperty); }
            set { SetValue(FractionProperty, value); }
        }

        public FontVariants Variants
        {
            get { return (FontVariants) GetValue(VariantsProperty); }
            set { SetValue(VariantsProperty, value); }
        }

        public FontCapitals Capitals
        {
            get { return (FontCapitals) GetValue(CapitalsProperty); }
            set { SetValue(CapitalsProperty, value); }
        }

        public FontNumeralStyle NumeralStyle
        {
            get { return (FontNumeralStyle) GetValue(NumeralStyleProperty); }
            set { SetValue(NumeralStyleProperty, value); }
        }

        public FontNumeralAlignment NumeralAlignment
        {
            get { return (FontNumeralAlignment) GetValue(NumeralAlignmentProperty); }
            set { SetValue(NumeralAlignmentProperty, value); }
        }

        public FontEastAsianWidths EastAsianWidths
        {
            get { return (FontEastAsianWidths) GetValue(EastAsianWidthsProperty); }
            set { SetValue(EastAsianWidthsProperty, value); }
        }

        public FontEastAsianLanguage EastAsianLanguage
        {
            get { return (FontEastAsianLanguage) GetValue(EastAsianLanguageProperty); }
            set { SetValue(EastAsianLanguageProperty, value); }
        }

        public int AnnotationAlternates
        {
            get { return (int) GetValue(AnnotationAlternatesProperty); }
            set { SetValue(AnnotationAlternatesProperty, value); }
        }

        public int StandardSwashes
        {
            get { return (int) GetValue(StandardSwashesProperty); }
            set { SetValue(StandardSwashesProperty, value); }
        }

        public int ContextualSwashes
        {
            get { return (int) GetValue(ContextualSwashesProperty); }
            set { SetValue(ContextualSwashesProperty, value); }
        }

        public int StylisticAlternates
        {
            get { return (int) GetValue(StylisticAlternatesProperty); }
            set { SetValue(StylisticAlternatesProperty, value); }
        }

        private static void TypographicPropertyChangedCallback(DependencyObject obj,
                                                               DependencyPropertyChangedEventArgs e)
        {
            var chooser = obj as SelectFontDialog;
            if (chooser != null)
            {
                chooser.InvalidatePreview();
            }
        }

        #endregion

        #region Other dependency properties

        public static readonly DependencyProperty SelectedFontFamilyProperty = RegisterFontProperty(
            "SelectedFontFamily",
            TextBlock.FontFamilyProperty,
            SelectedFontFamilyChangedCallback
            );

        public static readonly DependencyProperty SelectedFontSizeProperty = RegisterFontProperty(
            "SelectedFontSize",
            TextBlock.FontSizeProperty,
            SelectedFontSizeChangedCallback
            );

        public static readonly DependencyProperty SelectedFontStretchProperty = RegisterFontProperty(
            "SelectedFontStretch",
            TextBlock.FontStretchProperty,
            SelectedTypefaceChangedCallback
            );

        public static readonly DependencyProperty SelectedFontStyleProperty = RegisterFontProperty(
            "SelectedFontStyle",
            TextBlock.FontStyleProperty,
            SelectedTypefaceChangedCallback
            );

        public static readonly DependencyProperty SelectedFontWeightProperty = RegisterFontProperty(
            "SelectedFontWeight",
            TextBlock.FontWeightProperty,
            SelectedTypefaceChangedCallback
            );

        public static readonly DependencyProperty SelectedTextDecorationsProperty = RegisterFontProperty(
            "SelectedTextDecorations",
            TextBlock.TextDecorationsProperty,
            SelectedTextDecorationsChangedCallback
            );

        public FontFamily SelectedFontFamily
        {
            get { return GetValue(SelectedFontFamilyProperty) as FontFamily; }
            set { SetValue(SelectedFontFamilyProperty, value); }
        }

        public FontWeight SelectedFontWeight
        {
            get { return (FontWeight) GetValue(SelectedFontWeightProperty); }
            set { SetValue(SelectedFontWeightProperty, value); }
        }

        public FontStyle SelectedFontStyle
        {
            get { return (FontStyle) GetValue(SelectedFontStyleProperty); }
            set { SetValue(SelectedFontStyleProperty, value); }
        }

        public FontStretch SelectedFontStretch
        {
            get { return (FontStretch) GetValue(SelectedFontStretchProperty); }
            set { SetValue(SelectedFontStretchProperty, value); }
        }

        public double SelectedFontSize
        {
            get { return (double) GetValue(SelectedFontSizeProperty); }
            set { SetValue(SelectedFontSizeProperty, value); }
        }

        public TextDecorationCollection SelectedTextDecorations
        {
            get { return GetValue(SelectedTextDecorationsProperty) as TextDecorationCollection; }
            set { SetValue(SelectedTextDecorationsProperty, value); }
        }

        private static void SelectedFontFamilyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((SelectFontDialog)obj).OnSelectedFontFamilyChanged(e.NewValue as FontFamily);
        }

        private static void SelectedTypefaceChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((SelectFontDialog)obj).InvalidateTypefaceListSelection();
        }

        private static void SelectedFontSizeChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((SelectFontDialog)obj).OnSelectedFontSizeChanged((double)(e.NewValue));
        }

        private static void SelectedTextDecorationsChangedCallback(DependencyObject obj,
                                                                   DependencyPropertyChangedEventArgs e)
        {
            var chooser = (SelectFontDialog)obj;
            chooser.OnTextDecorationsChanged();
        }

        #endregion

        #region Dependency property helper functions

        // Helper function for registering typographic dependency properties with property-specific sample text.
        private static DependencyProperty RegisterTypographicProperty(DependencyProperty targetProperty,
                                                                      string sampleTextTag)
        {
            Type t = targetProperty.PropertyType;

            TypographyFeaturePage featurePage = (t == typeof (bool))
                                                    ? TypographyFeaturePage.BooleanFeaturePage
                                                    :
                                                        (t == typeof (int))
                                                            ? TypographyFeaturePage.IntegerFeaturePage
                                                            :
                                                                new TypographyFeaturePage(t);

            return DependencyProperty.Register(
                targetProperty.Name,
                t,
                typeof(SelectFontDialog),
                new TypographicPropertyMetadata(
                    targetProperty.DefaultMetadata.DefaultValue,
                    targetProperty,
                    featurePage,
                    sampleTextTag
                    )
                );
        }

        // Helper function for registering typographic dependency properties with default sample text for the type.
        private static DependencyProperty RegisterTypographicProperty(DependencyProperty targetProperty)
        {
            return RegisterTypographicProperty(targetProperty, null);
        }

        // Helper function for registering font chooser dependency properties other than typographic properties.
        private static DependencyProperty RegisterFontProperty(
            string propertyName,
            DependencyProperty targetProperty,
            PropertyChangedCallback changeCallback
            )
        {
            return DependencyProperty.Register(
                propertyName,
                targetProperty.PropertyType,
                typeof(SelectFontDialog),
                new FontPropertyMetadata(
                    targetProperty.DefaultMetadata.DefaultValue,
                    changeCallback,
                    targetProperty
                    )
                );
        }

        #endregion

        #region Dependency property tables

        // Array of all font chooser dependency properties
        private static readonly DependencyProperty[] _chooserProperties = new[]
                                                                              {
                                                                                  // typography properties
                                                                                  StandardLigaturesProperty,
                                                                                  ContextualLigaturesProperty,
                                                                                  DiscretionaryLigaturesProperty,
                                                                                  HistoricalLigaturesProperty,
                                                                                  ContextualAlternatesProperty,
                                                                                  HistoricalFormsProperty,
                                                                                  KerningProperty,
                                                                                  CapitalSpacingProperty,
                                                                                  CaseSensitiveFormsProperty,
                                                                                  SlashedZeroProperty,
                                                                                  MathematicalGreekProperty,
                                                                                  EastAsianExpertFormsProperty,
                                                                                  FractionProperty,
                                                                                  VariantsProperty,
                                                                                  CapitalsProperty,
                                                                                  NumeralStyleProperty,
                                                                                  NumeralAlignmentProperty,
                                                                                  EastAsianWidthsProperty,
                                                                                  EastAsianLanguageProperty,
                                                                                  AnnotationAlternatesProperty,
                                                                                  StandardSwashesProperty,
                                                                                  ContextualSwashesProperty,
                                                                                  StylisticAlternatesProperty,
                                                                                  // other properties
                                                                                  SelectedFontFamilyProperty,
                                                                                  SelectedFontWeightProperty,
                                                                                  SelectedFontStyleProperty,
                                                                                  SelectedFontStretchProperty,
                                                                                  SelectedFontSizeProperty,
                                                                                  SelectedTextDecorationsProperty
                                                                              };

        #endregion

        #region Property change handlers

        // Handle changes to the SelectedFontFamily property
        private void OnSelectedFontFamilyChanged(FontFamily family)
        {
            // If the family list is not valid do nothing for now. 
            // We'll be called again after the list is initialized.
            if (_familyListValid)
            {
                // Select the family in the list; this will return null if the family is not in the list.
                FontFamilyListItem item = SelectFontFamilyListItem(family);

                // Set the text box to the family name, if it isn't already.
                string displayName = (item != null) ? item.ToString() : FontFamilyListItem.GetDisplayName(family);
                if (string.Compare(fontFamilyTextBox.Text, displayName, true, CultureInfo.CurrentCulture) != 0)
                {
                    fontFamilyTextBox.Text = displayName;
                }

                // The typeface list is no longer valid; update it in the background to improve responsiveness.
                InvalidateTypefaceList();
            }
        }

        // Handle changes to the SelectedFontSize property
        private void OnSelectedFontSizeChanged(double sizeInPixels)
        {
            // Select the list item, if the size is in the list.
            double sizeInPoints = FontSizeListItem.PixelsToPoints(sizeInPixels);
            if (!SelectListItem(sizeList, sizeInPoints))
            {
                sizeList.SelectedIndex = -1;
            }

            // Set the text box contents if it doesn't already match the current size.
            double textBoxValue;
            if (!double.TryParse(sizeTextBox.Text, out textBoxValue) ||
                !FontSizeListItem.FuzzyEqual(textBoxValue, sizeInPoints))
            {
                sizeTextBox.Text = sizeInPoints.ToString();
            }

            // Schedule background updates.
            InvalidatePreview();
        }

        // Handle changes to any of the text decoration properties.
        private void OnTextDecorationsChanged()
        {
            bool underline = false;
            bool baseline = false;
            bool strikethrough = false;
            bool overline = false;

            TextDecorationCollection textDecorations = SelectedTextDecorations;
            if (textDecorations != null)
            {
                foreach (TextDecoration td in textDecorations)
                {
                    switch (td.Location)
                    {
                        case TextDecorationLocation.Underline:
                            underline = true;
                            break;
                        case TextDecorationLocation.Baseline:
                            baseline = true;
                            break;
                        case TextDecorationLocation.Strikethrough:
                            strikethrough = true;
                            break;
                        case TextDecorationLocation.OverLine:
                            overline = true;
                            break;
                    }
                }
            }

            underlineCheckBox.IsChecked = underline;
            baselineCheckBox.IsChecked = baseline;
            strikethroughCheckBox.IsChecked = strikethrough;
            overlineCheckBox.IsChecked = overline;

            // Schedule background updates.
            InvalidatePreview();
        }

        #endregion

        #region Background update logic

        // Schedule background initialization of the font famiy list.
        private void InvalidateFontFamilyList()
        {
            if (_familyListValid)
            {
                InvalidateTypefaceList();

                fontFamilyList.Items.Clear();
                fontFamilyTextBox.Clear();
                _familyListValid = false;

                ScheduleUpdate();
            }
        }

        // Schedule background initialization of the typeface list.
        private void InvalidateTypefaceList()
        {
            if (_typefaceListValid)
            {
                typefaceList.Items.Clear();
                _typefaceListValid = false;

                ScheduleUpdate();
            }
        }

        // Schedule background selection of the current typeface list item.
        private void InvalidateTypefaceListSelection()
        {
            if (_typefaceListSelectionValid)
            {
                _typefaceListSelectionValid = false;
                ScheduleUpdate();
            }
        }

        // Mark all the tabs as invalid and schedule background initialization of the current tab.
        private void InvalidateTabs()
        {
            ScheduleUpdate();
        }

        // Schedule background initialization of the preview control.
        private void InvalidatePreview()
        {
            if (_previewValid)
            {
                _previewValid = false;
                ScheduleUpdate();
            }
        }

        // Schedule background initialization.
        private void ScheduleUpdate()
        {
            if (!_updatePending)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new UpdateCallback(OnUpdate));
                _updatePending = true;
            }
        }

        // Dispatcher callback that performs background initialization.
        private void OnUpdate()
        {
            _updatePending = false;

            if (!_familyListValid)
            {
                // Initialize the font family list.
                InitializeFontFamilyList();
                _familyListValid = true;
                OnSelectedFontFamilyChanged(SelectedFontFamily);

                // Defer any other initialization until later.
                ScheduleUpdate();
            }
            else if (!_typefaceListValid)
            {
                // Initialize the typeface list.
                InitializeTypefaceList();
                _typefaceListValid = true;

                // Select the current typeface in the list.
                InitializeTypefaceListSelection();
                _typefaceListSelectionValid = true;

                // Defer any other initialization until later.
                ScheduleUpdate();
            }
            else if (!_typefaceListSelectionValid)
            {
                // Select the current typeface in the list.
                InitializeTypefaceListSelection();
                _typefaceListSelectionValid = true;

                // Defer any other initialization until later.
                ScheduleUpdate();
            }
            else
            {
                // Perform any remaining initialization.
                if (!_previewValid)
                {
                    // Initialize the preview control.
                    InitializePreview();
                    _previewValid = true;
                }
            }
        }

        #endregion

        #region Content initialization

        private void InitializeFontFamilyList()
        {
            ICollection<FontFamily> familyCollection = FontFamilyCollection;
            if (familyCollection != null)
            {
                var items = new FontFamilyListItem[familyCollection.Count];

                int i = 0;

                foreach (FontFamily family in familyCollection)
                {
                    items[i++] = new FontFamilyListItem(family);
                }

                Array.Sort(items);

                foreach (FontFamilyListItem item in items)
                {
                    fontFamilyList.Items.Add(item);
                }
            }
        }

        private void InitializeTypefaceList()
        {
            FontFamily family = SelectedFontFamily;
            if (family != null)
            {
                ICollection<Typeface> faceCollection = family.GetTypefaces();

                var items = new TypefaceListItem[faceCollection.Count];

                int i = 0;

                foreach (Typeface face in faceCollection)
                {
                    items[i++] = new TypefaceListItem(face);
                }

                Array.Sort(items);

                foreach (TypefaceListItem item in items)
                {
                    typefaceList.Items.Add(item);
                }
            }
        }

        private void InitializeTypefaceListSelection()
        {
            // If the typeface list is not valid, do nothing for now.
            // We'll be called again after the list is initialized.
            if (_typefaceListValid)
            {
                var typeface = new Typeface(SelectedFontFamily, SelectedFontStyle, SelectedFontWeight,
                                            SelectedFontStretch);

                // Select the typeface in the list.
                SelectTypefaceListItem(typeface);

                // Schedule background updates.
                InvalidateTabs();
                InvalidatePreview();
            }
        }

        private void InitializePreview()
        {
            ApplyPropertiesToObject(previewTextBox);
        }

        #endregion

        #region List box helpers

        // Update font family list based on selection.
        // Return list item if there's an exact match, or null if not.
        private FontFamilyListItem SelectFontFamilyListItem(string displayName)
        {
            var listItem = fontFamilyList.SelectedItem as FontFamilyListItem;
            if (listItem != null &&
                string.Compare(listItem.ToString(), displayName, true, CultureInfo.CurrentCulture) == 0)
            {
                // Already selected
                return listItem;
            }
            if (SelectListItem(fontFamilyList, displayName))
            {
                // Exact match found
                return fontFamilyList.SelectedItem as FontFamilyListItem;
            }
            // Not in the list
            return null;
        }

        // Update font family list based on selection.
        // Return list item if there's an exact match, or null if not.
        private FontFamilyListItem SelectFontFamilyListItem(FontFamily family)
        {
            var listItem = fontFamilyList.SelectedItem as FontFamilyListItem;
            if (listItem != null && listItem.FontFamily.Equals(family))
            {
                // Already selected
                return listItem;
            }
            if (SelectListItem(fontFamilyList, FontFamilyListItem.GetDisplayName(family)))
            {
                // Exact match found
                return fontFamilyList.SelectedItem as FontFamilyListItem;
            }
            // Not in the list
            return null;
        }

        // Update typeface list based on selection.
        // Return list item if there's an exact match, or null if not.
        private TypefaceListItem SelectTypefaceListItem(Typeface typeface)
        {
            var listItem = typefaceList.SelectedItem as TypefaceListItem;
            if (listItem != null && listItem.Typeface.Equals(typeface))
            {
                // Already selected
                return listItem;
            }
            if (SelectListItem(typefaceList, new TypefaceListItem(typeface)))
            {
                // Exact match found
                return typefaceList.SelectedItem as TypefaceListItem;
            }
            // Not in list
            return null;
        }

        // Update list based on selection.
        // Return true if there's an exact match, or false if not.
        private bool SelectListItem(ListBox list, object value)
        {
            ItemCollection itemList = list.Items;

            // Perform a binary search for the item.
            int first = 0;
            int limit = itemList.Count;

            while (first < limit)
            {
                int i = first + (limit - first)/2;
                var item = (IComparable) (itemList[i]);
                int comparison = item.CompareTo(value);
                if (comparison < 0)
                {
                    // Value must be after i
                    first = i + 1;
                }
                else if (comparison > 0)
                {
                    // Value must be before i
                    limit = i;
                }
                else
                {
                    // Exact match; select the item.
                    list.SelectedIndex = i;
                    itemList.MoveCurrentToPosition(i);
                    list.ScrollIntoView(itemList[i]);
                    return true;
                }
            }

            // Not an exact match; move current position to the nearest item but don't select it.
            if (itemList.Count > 0)
            {
                int i = Math.Min(first, itemList.Count - 1);
                itemList.MoveCurrentToPosition(i);
                list.ScrollIntoView(itemList[i]);
            }

            return false;
        }

        // Logic to handle UP and DOWN arrow keys in the text box associated with a list.
        // Behavior is similar to a Win32 combo box.
        private void OnComboBoxPreviewKeyDown(TextBox textBox, ListBox listBox, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    // Move up from the current position.
                    MoveListPosition(listBox, -1);
                    e.Handled = true;
                    break;

                case Key.Down:
                    // Move down from the current position, unless the item at the current position is
                    // not already selected in which case select it.
                    if (listBox.Items.CurrentPosition == listBox.SelectedIndex)
                    {
                        MoveListPosition(listBox, +1);
                    }
                    else
                    {
                        MoveListPosition(listBox, 0);
                    }
                    e.Handled = true;
                    break;
            }
        }

        private void MoveListPosition(ListBox listBox, int distance)
        {
            int i = listBox.Items.CurrentPosition + distance;
            if (i >= 0 && i < listBox.Items.Count)
            {
                listBox.Items.MoveCurrentToPosition(i);
                listBox.SelectedIndex = i;
                listBox.ScrollIntoView(listBox.Items[i]);
            }
        }

        #endregion
    }

    internal class TypefaceListItem : TextBlock, IComparable
    {
        private readonly string _displayName;
        private readonly bool _simulated;

        public TypefaceListItem(Typeface typeface)
        {
            _displayName = GetDisplayName(typeface);
            _simulated = typeface.IsBoldSimulated || typeface.IsObliqueSimulated;

            FontFamily = typeface.FontFamily;
            FontWeight = typeface.Weight;
            FontStyle = typeface.Style;
            FontStretch = typeface.Stretch;

            string itemLabel = _displayName;

            if (_simulated)
            {
                string formatString = AnjLab.FX.Wpf.Properties.Resources.ResourceManager.GetString(
                    "simulated",
                    CultureInfo.CurrentUICulture
                    );
                itemLabel = string.Format(formatString, itemLabel);
            }

            Text = itemLabel;
            ToolTip = itemLabel;

            // In the case of symbol font, apply the default message font to the text so it can be read.
            if (FontFamilyListItem.IsSymbolFont(typeface.FontFamily))
            {
                var range = new TextRange(ContentStart, ContentEnd);
                range.ApplyPropertyValue(FontFamilyProperty, SystemFonts.MessageFontFamily);
            }
        }

        public Typeface Typeface
        {
            get { return new Typeface(FontFamily, FontStyle, FontWeight, FontStretch); }
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            var item = obj as TypefaceListItem;
            if (item == null)
            {
                return -1;
            }

            // Sort all simulated faces after all non-simulated faces.
            if (_simulated != item._simulated)
            {
                return _simulated ? 1 : -1;
            }

            // If weight differs then sort based on weight (lightest first).
            int difference = FontWeight.ToOpenTypeWeight() - item.FontWeight.ToOpenTypeWeight();
            if (difference != 0)
            {
                return difference > 0 ? 1 : -1;
            }

            // If style differs then sort based on style (Normal, Italic, then Oblique).
            FontStyle thisStyle = FontStyle;
            FontStyle otherStyle = item.FontStyle;

            if (thisStyle != otherStyle)
            {
                if (thisStyle == FontStyles.Normal)
                    // This item is normal style and should come first.
                    return -1;
                if (otherStyle == FontStyles.Normal)
                    // The other item is normal style and should come first.
                    return 1;
                // Neither is normal so sort italic before oblique.
                return (thisStyle == FontStyles.Italic) ? -1 : 1;
            }

            // If stretch differs then sort based on stretch (Normal first, then numerically).
            FontStretch thisStretch = FontStretch;
            FontStretch otherStretch = item.FontStretch;

            if (thisStretch != otherStretch)
            {
                if (thisStretch == FontStretches.Normal)
                    // This item is normal stretch and should come first.
                    return -1;
                if (otherStretch == FontStretches.Normal)
                    // The other item is normal stretch and should come first.
                    return 1;
                // Neither is normal so sort numerically.
                return thisStretch.ToOpenTypeStretch() < otherStretch.ToOpenTypeStretch() ? -1 : 0;
            }

            // They're the same.
            return 0;
        }

        #endregion

        public override string ToString()
        {
            return _displayName;
        }

        internal static string GetDisplayName(Typeface typeface)
        {
            return NameDictionaryHelper.GetDisplayName(typeface.FaceNames);
        }
    }

    internal static class NameDictionaryHelper
    {
        public static string GetDisplayName(LanguageSpecificStringDictionary nameDictionary)
        {
            // Look up the display name based on the UI culture, which is the same culture
            // used for resource loading.
            XmlLanguage userLanguage = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

            // Look for an exact match.
            string name;
            if (nameDictionary.TryGetValue(userLanguage, out name))
            {
                return name;
            }

            // No exact match; return the name for the most closely related language.
            int bestRelatedness = -1;
            string bestName = string.Empty;

            foreach (KeyValuePair<XmlLanguage, string> pair in nameDictionary)
            {
                int relatedness = GetRelatedness(pair.Key, userLanguage);
                if (relatedness > bestRelatedness)
                {
                    bestRelatedness = relatedness;
                    bestName = pair.Value;
                }
            }

            return bestName;
        }

        public static string GetDisplayName(IDictionary<CultureInfo, string> nameDictionary)
        {
            // Look for an exact match.
            string name;
            if (nameDictionary.TryGetValue(CultureInfo.CurrentUICulture, out name))
            {
                return name;
            }

            // No exact match; return the name for the most closely related language.
            int bestRelatedness = -1;
            string bestName = string.Empty;

            XmlLanguage userLanguage = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

            foreach (KeyValuePair<CultureInfo, string> pair in nameDictionary)
            {
                int relatedness = GetRelatedness(XmlLanguage.GetLanguage(pair.Key.IetfLanguageTag), userLanguage);
                if (relatedness > bestRelatedness)
                {
                    bestRelatedness = relatedness;
                    bestName = pair.Value;
                }
            }

            return bestName;
        }

        private static int GetRelatedness(XmlLanguage keyLang, XmlLanguage userLang)
        {
            try
            {
                // Get equivalent cultures.
                CultureInfo keyCulture = CultureInfo.GetCultureInfoByIetfLanguageTag(keyLang.IetfLanguageTag);
                CultureInfo userCulture = CultureInfo.GetCultureInfoByIetfLanguageTag(userLang.IetfLanguageTag);
                if (!userCulture.IsNeutralCulture)
                {
                    userCulture = userCulture.Parent;
                }

                // If the key is a prefix or parent of the user language it's a good match.
                if (IsPrefixOf(keyLang.IetfLanguageTag, userLang.IetfLanguageTag) || userCulture.Equals(keyCulture))
                {
                    return 2;
                }

                // If the key and user language share a common prefix or parent neutral culture, it's a reasonable match.
                if (IsPrefixOf(TrimSuffix(userLang.IetfLanguageTag), keyLang.IetfLanguageTag) || userCulture.Equals(keyCulture.Parent))
                {
                    return 1;
                }
            }
            catch (ArgumentException)
            {
                // Language tag with no corresponding CultureInfo.
            }

            // They're unrelated languages.
            return 0;
        }

        private static string TrimSuffix(string tag)
        {
            int i = tag.LastIndexOf('-');
            if (i > 0)
            {
                return tag.Substring(0, i);
            }
            return tag;
        }

        private static bool IsPrefixOf(string prefix, string tag)
        {
            return prefix.Length < tag.Length &&
                   tag[prefix.Length] == '-' &&
                   string.CompareOrdinal(prefix, 0, tag, 0, prefix.Length) == 0;
        }
    }

    internal class FontSizeListItem : TextBlock, IComparable
    {
        private readonly double _sizeInPoints;

        public FontSizeListItem(double sizeInPoints)
        {
            _sizeInPoints = sizeInPoints;
            Text = sizeInPoints.ToString();
        }

        public double SizeInPoints
        {
            get { return _sizeInPoints; }
        }

        public double SizeInPixels
        {
            get { return PointsToPixels(_sizeInPoints); }
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            double value;

            if (obj is double)
            {
                value = (double)obj;
            }
            else
            {
                if (!double.TryParse(obj.ToString(), out value))
                {
                    return 1;
                }
            }

            return
                FuzzyEqual(_sizeInPoints, value)
                    ? 0
                    :
                        (_sizeInPoints < value) ? -1 : 1;
        }

        #endregion

        public override string ToString()
        {
            return _sizeInPoints.ToString();
        }

        public static bool FuzzyEqual(double a, double b)
        {
            return Math.Abs(a - b) < 0.01;
        }

        public static double PointsToPixels(double value)
        {
            return value * (96.0 / 72.0);
        }

        public static double PixelsToPoints(double value)
        {
            return value * (72.0 / 96.0);
        }
    }

    internal class FontFamilyListItem : TextBlock, IComparable
    {
        private readonly string _displayName;

        public FontFamilyListItem(FontFamily fontFamily)
        {
            _displayName = GetDisplayName(fontFamily);

            FontFamily = fontFamily;
            Text = _displayName;
            ToolTip = _displayName;

            // In the case of symbol font, apply the default message font to the text so it can be read.
            if (IsSymbolFont(fontFamily))
            {
                var range = new TextRange(ContentStart, ContentEnd);
                range.ApplyPropertyValue(FontFamilyProperty, SystemFonts.MessageFontFamily);
            }
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return string.Compare(_displayName, obj.ToString(), true, CultureInfo.CurrentCulture);
        }

        #endregion

        public override string ToString()
        {
            return _displayName;
        }

        internal static bool IsSymbolFont(FontFamily fontFamily)
        {
            foreach (Typeface typeface in fontFamily.GetTypefaces())
            {
                GlyphTypeface face;
                if (typeface.TryGetGlyphTypeface(out face))
                {
                    return face.Symbol;
                }
            }
            return false;
        }

        internal static string GetDisplayName(FontFamily family)
        {
            return NameDictionaryHelper.GetDisplayName(family.FamilyNames);
        }
    }
}