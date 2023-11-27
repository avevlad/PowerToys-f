// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FancyZonesEditorCommon.Data;
using Microsoft.FancyZonesEditor.UITests.Utils;
using Microsoft.FancyZonesEditor.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using static FancyZonesEditorCommon.Data.AppliedLayouts;
using static FancyZonesEditorCommon.Data.DefaultLayouts;
using static FancyZonesEditorCommon.Data.EditorParameters;
using static FancyZonesEditorCommon.Data.LayoutTemplates;

namespace Microsoft.FancyZonesEditor.UITests
{
    [TestClass]
    public class TemplateLayoutsTests
    {
        private static readonly TemplateLayoutsListWrapper Layouts = new TemplateLayoutsListWrapper
        {
            LayoutTemplates = new List<TemplateLayoutWrapper>
            {
                new TemplateLayoutWrapper
                {
                    Type = Constants.TemplateLayoutJsonTags[Constants.TemplateLayout.Empty],
                },
                new TemplateLayoutWrapper
                {
                    Type = Constants.TemplateLayoutJsonTags[Constants.TemplateLayout.Focus],
                    ZoneCount = 10,
                },
                new TemplateLayoutWrapper
                {
                    Type = Constants.TemplateLayoutJsonTags[Constants.TemplateLayout.Rows],
                    ZoneCount = 2,
                    ShowSpacing = true,
                    Spacing = 10,
                    SensitivityRadius = 10,
                },
                new TemplateLayoutWrapper
                {
                    Type = Constants.TemplateLayoutJsonTags[Constants.TemplateLayout.Columns],
                    ZoneCount = 2,
                    ShowSpacing = true,
                    Spacing = 20,
                    SensitivityRadius = 20,
                },
                new TemplateLayoutWrapper
                {
                    Type = Constants.TemplateLayoutJsonTags[Constants.TemplateLayout.Grid],
                    ZoneCount = 4,
                    ShowSpacing = false,
                    Spacing = 10,
                    SensitivityRadius = 30,
                },
                new TemplateLayoutWrapper
                {
                    Type = Constants.TemplateLayoutJsonTags[Constants.TemplateLayout.PriorityGrid],
                    ZoneCount = 3,
                    ShowSpacing = true,
                    Spacing = 1,
                    SensitivityRadius = 40,
                },
            },
        };

        private static TestContext? _context;
        private static FancyZonesEditorSession? _session;
        private static IOTestHelper? _editorParamsIOHelper;
        private static IOTestHelper? _templatesIOHelper;
        private static IOTestHelper? _defaultLayoutsIOHelper;
        private static IOTestHelper? _appliedLayoutsIOHelper;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _context = testContext;

            EditorParameters editorParameters = new EditorParameters();
            ParamsWrapper parameters = new ParamsWrapper
            {
                ProcessId = 1,
                SpanZonesAcrossMonitors = false,
                Monitors = new List<NativeMonitorDataWrapper>
                {
                    new NativeMonitorDataWrapper
                    {
                        Monitor = "monitor-1",
                        MonitorInstanceId = "instance-id-1",
                        MonitorSerialNumber = "serial-number-1",
                        MonitorNumber = 1,
                        VirtualDesktop = "{FF34D993-73F3-4B8C-AA03-73730A01D6A8}",
                        Dpi = 192,
                        LeftCoordinate = 0,
                        TopCoordinate = 0,
                        WorkAreaHeight = 1040,
                        WorkAreaWidth = 1920,
                        MonitorHeight = 1080,
                        MonitorWidth = 1920,
                        IsSelected = true,
                    },
                },
            };
            _editorParamsIOHelper = new IOTestHelper(editorParameters.File);
            _editorParamsIOHelper.WriteData(editorParameters.Serialize(parameters));

            LayoutTemplates layoutTemplates = new LayoutTemplates();
            _templatesIOHelper = new IOTestHelper(layoutTemplates.File);
            _templatesIOHelper.WriteData(layoutTemplates.Serialize(Layouts));

            // Default layouts should match templates
            DefaultLayouts defaultLayouts = new DefaultLayouts();
            DefaultLayoutsListWrapper defaultLayoutsList = new DefaultLayoutsListWrapper
            {
                DefaultLayouts = new List<DefaultLayoutWrapper>
                {
                    new DefaultLayoutWrapper
                    {
                        MonitorConfiguration = MonitorConfigurationType.Vertical.ToString(),
                        Layout = new DefaultLayoutWrapper.LayoutWrapper
                        {
                            Type = Constants.TemplateLayoutJsonTags[Constants.TemplateLayout.Rows],
                            ZoneCount = 2,
                            ShowSpacing = true,
                            Spacing = 10,
                            SensitivityRadius = 10,
                        },
                    },
                    new DefaultLayoutWrapper
                    {
                        MonitorConfiguration = MonitorConfigurationType.Horizontal.ToString(),
                        Layout = new DefaultLayoutWrapper.LayoutWrapper
                        {
                            Type = Constants.TemplateLayoutJsonTags[Constants.TemplateLayout.PriorityGrid],
                            ZoneCount = 3,
                            ShowSpacing = true,
                            Spacing = 1,
                            SensitivityRadius = 40,
                        },
                    },
                },
            };
            _defaultLayoutsIOHelper = new IOTestHelper(defaultLayouts.File);
            _defaultLayoutsIOHelper.WriteData(defaultLayouts.Serialize(defaultLayoutsList));

            // Make sure applied layouts don't replate template settings
            AppliedLayouts appliedLayouts = new AppliedLayouts();
            AppliedLayoutsListWrapper appliedLayoutsList = new AppliedLayoutsListWrapper
            {
                AppliedLayouts = new List<AppliedLayoutWrapper>
                {
                    new AppliedLayoutWrapper
                    {
                        Device = new AppliedLayoutWrapper.DeviceIdWrapper
                        {
                            Monitor = "monitor-1",
                            MonitorInstance = "instance-id-1",
                            MonitorNumber = 1,
                            SerialNumber = "serial-number-1",
                            VirtualDesktop = "{FF34D993-73F3-4B8C-AA03-73730A01D6A8}",
                        },
                        AppliedLayout = new AppliedLayoutWrapper.LayoutWrapper
                        {
                            Type = Constants.TemplateLayoutJsonTags[Constants.TemplateLayout.PriorityGrid],
                            ZoneCount = 3,
                            ShowSpacing = true,
                            Spacing = 1,
                            SensitivityRadius = 40,
                        },
                    },
                },
            };
            _appliedLayoutsIOHelper = new IOTestHelper(appliedLayouts.File);
            _appliedLayoutsIOHelper.WriteData(appliedLayouts.Serialize(appliedLayoutsList));
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _editorParamsIOHelper?.RestoreData();
            _templatesIOHelper?.RestoreData();
            _defaultLayoutsIOHelper?.RestoreData();
            _appliedLayoutsIOHelper?.RestoreData();

            _context = null;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            LayoutTemplates layoutTemplates = new LayoutTemplates();
            _templatesIOHelper = new IOTestHelper(layoutTemplates.File);
            _templatesIOHelper.WriteData(layoutTemplates.Serialize(Layouts));

            // Default layouts should match templates
            DefaultLayouts defaultLayouts = new DefaultLayouts();
            DefaultLayoutsListWrapper defaultLayoutsList = new DefaultLayoutsListWrapper
            {
                DefaultLayouts = new List<DefaultLayoutWrapper>
                {
                    new DefaultLayoutWrapper
                    {
                        MonitorConfiguration = MonitorConfigurationType.Vertical.ToString(),
                        Layout = new DefaultLayoutWrapper.LayoutWrapper
                        {
                            Type = Constants.TemplateLayoutTypes[Constants.TemplateLayouts.Rows],
                            ZoneCount = 2,
                            ShowSpacing = true,
                            Spacing = 10,
                            SensitivityRadius = 10,
                        },
                    },
                    new DefaultLayoutWrapper
                    {
                        MonitorConfiguration = MonitorConfigurationType.Horizontal.ToString(),
                        Layout = new DefaultLayoutWrapper.LayoutWrapper
                        {
                            Type = Constants.TemplateLayoutTypes[Constants.TemplateLayouts.PriorityGrid],
                            ZoneCount = 3,
                            ShowSpacing = true,
                            Spacing = 1,
                            SensitivityRadius = 40,
                        },
                    },
                },
            };
            _defaultLayoutsIOHelper = new IOTestHelper(defaultLayouts.File);
            _defaultLayoutsIOHelper.WriteData(defaultLayouts.Serialize(defaultLayoutsList));

            _session = new FancyZonesEditorSession(_context!);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _session?.Close(_context!);

            _templatesIOHelper?.RestoreData();
            _defaultLayoutsIOHelper?.RestoreData();
        }

        [TestMethod]
        public void ZoneNumber_Initialize()
        {
            foreach (var (key, name) in TestConstants.TemplateLayoutNames)
            {
                if (key == Constants.TemplateLayout.Empty)
                {
                    continue;
                }

                _session?.Click_EditLayout(name);

                var slider = _session?.GetZoneCountSlider();
                var expected = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutJsonTags[key]).ZoneCount;
                Assert.AreEqual($"{expected}", slider?.Text);

                _session?.Click_Cancel();

                // let the dialog window close
                _session?.WaitFor(0.5f);
            }
        }

        [TestMethod]
        public void ZoneNumber_Save()
        {
            var type = Constants.TemplateLayouts.Columns;
            var layout = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]);
            var value = layout.ZoneCount;
            var expected = value - 1;
            _session?.Click_EditLayout(Constants.TemplateLayoutNames[type]);

            var slider = _session?.GetZoneCountSlider();
            slider?.SendKeys(Keys.Left);
            Assert.AreEqual($"{expected}", slider?.Text);

            _session?.Click_Save();

            // let the dialog window close
            _session?.WaitFor(0.5f);

            // verify the file
            var templateLayouts = new LayoutTemplates();
            var data = templateLayouts.Read(templateLayouts.File);
            var actual = data.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]).ZoneCount;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ZoneNumber_Cancel()
        {
            var type = Constants.TemplateLayouts.Rows;
            var layout = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]);
            var expected = layout.ZoneCount;
            _session?.Click_EditLayout(Constants.TemplateLayoutNames[type]);

            var slider = _session?.GetZoneCountSlider();
            slider?.SendKeys(Keys.Left);

            _session?.Click_Cancel();

            // let the dialog window close
            _session?.WaitFor(0.5f);

            // verify the file
            var templateLayouts = new LayoutTemplates();
            var data = templateLayouts.Read(templateLayouts.File);
            var actual = data.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]).ZoneCount;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void HighlightDistance_Initialize()
        {
            foreach (var (key, name) in Constants.TemplateLayoutNames)
            {
                if (key == Constants.TemplateLayout.Empty)
                {
                    continue;
                }

                _session?.Click_EditLayout(name);

                var slider = _session?.GetSensitivitySlider();
                var expected = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutJsonTags[key]).SensitivityRadius;
                Assert.AreEqual($"{expected}", slider?.Text);

                _session?.Click_Cancel();

                // let the dialog window close
                _session?.WaitFor(0.5f);
            }
        }

        [TestMethod]
        public void HighlightDistance_Save()
        {
            var type = Constants.TemplateLayouts.Focus;
            var layout = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]);
            var value = layout.SensitivityRadius;
            _session?.Click_EditLayout(Constants.TemplateLayoutNames[type]);

            var slider = _session?.GetSensitivitySlider();
            slider?.SendKeys(Keys.Right);

            var expected = value + 1; // one step right
            Assert.AreEqual($"{expected}", slider?.Text);

            _session?.Click_Save();

            // let the dialog window close
            _session?.WaitFor(0.5f);

            // verify the file
            var templateLayouts = new LayoutTemplates();
            var data = templateLayouts.Read(templateLayouts.File);
            var actual = data.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]).SensitivityRadius;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void HighlightDistance_Cancel()
        {
            var type = Constants.TemplateLayouts.Focus;
            var layout = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]);
            var expected = layout.SensitivityRadius;
            _session?.Click_EditLayout(Constants.TemplateLayoutNames[type]);

            var slider = _session?.GetSensitivitySlider();
            slider?.SendKeys(Keys.Right);
            _session?.Click_Cancel();

            // let the dialog window close
            _session?.WaitFor(0.5f);

            // verify the file
            var templateLayouts = new LayoutTemplates();
            var data = templateLayouts.Read(templateLayouts.File);
            var actual = data.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]).SensitivityRadius;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SpaceAroundZones_Initialize()
        {
            foreach (var (key, name) in Constants.TemplateLayoutNames)
            {
                if (key == Constants.TemplateLayout.Empty || key == Constants.TemplateLayout.Focus)
                {
                    // only for grid layouts
                    continue;
                }

                _session?.Click_EditLayout(name);

                var toggle = _session?.GetSpaceAroundZonesToggle();
                var slider = _session?.GetSpaceAroundZonesSlider();

                var spacingEnabled = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutJsonTags[key]).ShowSpacing;
                Assert.AreEqual(spacingEnabled, slider?.Enabled);

                var expected = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutJsonTags[key]).Spacing;
                Assert.AreEqual($"{expected}", slider?.Text);

                _session?.Click_Cancel();

                // let the dialog window close
                _session?.WaitFor(0.5f);
            }
        }

        [TestMethod]
        public void SpaceAroundZones_Slider_Save()
        {
            var type = Constants.TemplateLayouts.PriorityGrid;
            var layout = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]);
            var expected = layout.Spacing + 1;
            _session?.Click_EditLayout(Constants.TemplateLayoutNames[type]);

            var slider = _session?.GetSpaceAroundZonesSlider();
            slider?.SendKeys(Keys.Right);
            Assert.AreEqual($"{expected}", slider?.Text);

            _session?.Click_Save();

            // let the dialog window close
            _session?.WaitFor(0.5f);

            // verify the file
            var templateLayouts = new LayoutTemplates();
            var data = templateLayouts.Read(templateLayouts.File);
            var actual = data.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]).Spacing;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SpaceAroundZones_Slider_Cancel()
        {
            var type = Constants.TemplateLayouts.PriorityGrid;
            var layout = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]);
            var expected = layout.Spacing;
            _session?.Click_EditLayout(Constants.TemplateLayoutNames[type]);

            var slider = _session?.GetSpaceAroundZonesSlider();
            slider?.SendKeys(Keys.Right);
            Assert.AreEqual($"{expected + 1}", slider?.Text);

            _session?.Click_Cancel();

            // let the dialog window close
            _session?.WaitFor(0.5f);

            // verify the file
            var templateLayouts = new LayoutTemplates();
            var data = templateLayouts.Read(templateLayouts.File);
            var actual = data.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]).Spacing;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SpaceAroundZones_Toggle_Save()
        {
            var type = Constants.TemplateLayouts.PriorityGrid;
            var layout = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]);
            var expected = !layout.ShowSpacing;
            _session?.Click_EditLayout(Constants.TemplateLayoutNames[type]);

            var toggle = _session?.GetSpaceAroundZonesToggle();
            toggle?.Click();
            Assert.AreEqual(expected, toggle?.Selected);
            Assert.AreEqual(expected, _session?.GetSpaceAroundZonesSlider()?.Enabled);

            _session?.Click_Save();

            // let the dialog window close
            _session?.WaitFor(0.5f);

            // verify the file
            var templateLayouts = new LayoutTemplates();
            var data = templateLayouts.Read(templateLayouts.File);
            var actual = data.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]).ShowSpacing;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SpaceAroundZones_Toggle_Cancel()
        {
            var type = Constants.TemplateLayouts.PriorityGrid;
            var layout = Layouts.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]);
            var expected = layout.ShowSpacing;
            _session?.Click_EditLayout(Constants.TemplateLayoutNames[type]);

            var toggle = _session?.GetSpaceAroundZonesToggle();
            toggle?.Click();
            Assert.AreNotEqual(expected, toggle?.Selected);
            Assert.AreNotEqual(expected, _session?.GetSpaceAroundZonesSlider()?.Enabled);

            _session?.Click_Cancel();

            // let the dialog window close
            _session?.WaitFor(0.5f);

            // verify the file
            var templateLayouts = new LayoutTemplates();
            var data = templateLayouts.Read(templateLayouts.File);
            var actual = data.LayoutTemplates.Find(x => x.Type == Constants.TemplateLayoutTypes[type]).ShowSpacing;
            Assert.AreEqual(expected, actual);
        }
    }
}