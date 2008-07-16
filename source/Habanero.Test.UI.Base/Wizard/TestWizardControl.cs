//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Habanero.UI.Base;
using Habanero.UI.WebGUI;
using NUnit.Framework;

namespace Habanero.Test.UI.Base.Wizard
{
  
    public abstract class TestWizardControl
    {
        protected abstract IControlFactory GetControlFactory();

        //[TestFixture]
        //public class TestWizardControlWin : TestWizardControl
        //{
        //    protected override IControlFactory GetControlFactory()
        //    {
        //        return new Habanero.UI.Win.ControlFactoryWin();
        //    }
        //}

        [TestFixture]
        public class TestWizardControlGiz : TestWizardControl
        {
            protected override IControlFactory GetControlFactory()
            {
                return new Habanero.UI.WebGUI.ControlFactoryGizmox();
            }
        }

        private WizardControllerStub _controller;
        private IWizardControl _wizardControl;

        private string _message;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _controller = new WizardControllerStub();
            _wizardControl = GetControlFactory().CreateWizardControl(_controller);// new WizardControl(_controller);
        }
        [SetUp]
        public void SetupTest()
        {
            _controller.ControlForStep1.AllowMoveOn = true;
            _message = "";
            _wizardControl.Start();
            
        }
        //TODO: Tests for layout management?
        [TearDown]
        public void TearDownTest()
        {
        }

        [Test]
        public void TestConstructWizardControl()
        {
            //---------------Set up test pack-------------------
            WizardControllerStub wizardController = new WizardControllerStub();

            //---------------Execute Test ----------------------
            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(wizardController);
            SetWizardControlSize(wizardControl);

            //---------------Test Result -----------------------
            Assert.IsNotNull(wizardControl.PreviousButton);
            Assert.IsNotNull(wizardControl.NextButton);
            Assert.IsNotNull(wizardControl.CancelButton);
            Assert.Less(wizardControl.PreviousButton.Left, wizardControl.NextButton.Left);
            Assert.Less(wizardControl.NextButton.Left, wizardControl.CancelButton.Left);
            Assert.AreEqual(0, wizardControl.PreviousButton.TabIndex);
            Assert.AreEqual(1, wizardControl.NextButton.TabIndex);
            Assert.AreEqual(wizardControl.Height - wizardControl.NextButton.Height  - 62, wizardControl.WizardStepPanel.Height);
        }

        [Test]
        public void TestCancelFiresEvent()
        {
            //---------------Set up test pack-------------------
            WizardControllerStub wizardController = new WizardControllerStub();

            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(wizardController);
            
            //--------------Assert PreConditions----------------            
            Assert.IsFalse(wizardController.CancelButtonEventFired );
            //---------------Execute Test ----------------------

            wizardControl.CancelButton.PerformClick();
            //---------------Test Result -----------------------
            Assert.IsTrue(wizardController.CancelButtonEventFired);

        }

        private static void SetWizardControlSize(IWizardControl wizardControl)
        {
            wizardControl.Width = 310;
            wizardControl.Height = 412;
        }

        [Test]
        public void TestStart()
        {
            //Setup -----------------------------------------------------
            WizardControllerStub  wizardController = new WizardControllerStub();
            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(wizardController);
            //Execute ---------------------------------------------------
            wizardControl.Start();
            //Assert Results --------------------------------------------
            Assert.AreEqual("ControlForStep1", wizardControl.CurrentControl.Name);
            Assert.AreEqual(wizardController.ControlForStep1.Name, wizardControl.CurrentControl.Name);

        }

        [Test]
        public void TestHeaderLabelEnabledWhen_WizardStepTextSet()
        {
            //---------------Set up test pack-------------------
            WizardControllerStub wizardController = new WizardControllerStub();
            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(wizardController);
            SetWizardControlSize(wizardControl);
            wizardControl.Start();
            //--------------Assert PreConditions----------------            
            Assert.AreEqual("ControlForStep1", wizardControl.CurrentControl.Name);
            //Assert.IsFalse(wizardControl.HeadingLabel.Visible);
            Assert.AreEqual(wizardControl.Height - wizardControl.NextButton.Height - 62, wizardControl.WizardStepPanel.Height);
            //---------------Execute Test ----------------------
            wizardControl.Next();
            //---------------Test Result -----------------------
            IWizardStep currentStep = wizardController.GetCurrentStep();
            Assert.AreEqual("ControlForStep2", wizardControl.CurrentControl.Name);
            Assert.AreSame(currentStep, wizardControl.CurrentControl);
            Assert.IsTrue(((WizardStepStub)currentStep).IsInitialised);
            //Assert.IsTrue(wizardControl.HeadingLabel.Visible);
            //Assert.IsTrue(wizardControl.HeadingLabel.Text.Length > 0);
            //Assert.AreEqual(step.HeaderText, wizardControl.HeadingLabel.Text);
            //Assert.AreEqual(wizardControl.Height - wizardControl.NextButton.Height - 62 - wizardControl.HeadingLabel.Height, wizardControl.WizardStepPanel.Height);
        }
        [Test]
        public void TestHeaderLabelDisabledWhen_WizardStepTextSetBackToNull()
        {
            //---------------Set up test pack-------------------
            WizardControllerStub wizardController = new WizardControllerStub();
            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(wizardController);
            SetWizardControlSize(wizardControl);
            wizardControl.Start();
            wizardControl.Next();
            //--------------Assert PreConditions----------------            
            wizardController.GetCurrentStep();
            Assert.AreEqual("ControlForStep2", wizardControl.CurrentControl.Name);
            //Assert.IsTrue(wizardControl.HeadingLabel.Visible); //removed the label and am now putting the header on the form
            // due to problems with giz hiding the some wizard controls that where double clicked
            //Assert.IsTrue(wizardControl.HeadingLabel.Text.Length > 0);
            //Assert.AreEqual(step.HeaderText, wizardControl.HeadingLabel.Text);
            //Assert.AreEqual(wizardControl.Height - wizardControl.NextButton.Height - 62 - wizardControl.HeadingLabel.Height, wizardControl.WizardStepPanel.Height);

            //---------------Execute Test ----------------------

            wizardControl.Previous();
            //---------------Test Result -----------------------
            wizardController.GetCurrentStep();
            Assert.AreEqual("ControlForStep1", wizardControl.CurrentControl.Name);
            //Assert.IsFalse(wizardControl.HeadingLabel.Visible);
            //Assert.IsFalse(wizardControl.HeadingLabel.Text.Length > 0);
            //Assert.AreEqual(step.HeaderText, wizardControl.HeadingLabel.Text);
            Assert.AreEqual(wizardControl.Height - wizardControl.NextButton.Height - 62, wizardControl.WizardStepPanel.Height);
        }

        //TODO: Tab indexes are not being set up correctly in Giz with the flow layout manager
        // right alignment
        [Test]
        public void Test_SetWizardController_CallsStart()
        {
            //Setup ----------------------------------------------------
            WizardControllerStub wizardController = new WizardControllerStub();
            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(_controller);
            wizardControl.Width = 300;
            //Execute ---------------------------------------------------
            wizardControl.WizardController = wizardController;
            //Assert Results --------------------------------------------
            Assert.AreEqual("ControlForStep1", wizardControl.CurrentControl.Name);
            Assert.AreEqual(wizardController.ControlForStep1.Name, wizardControl.CurrentControl.Name);
            Assert.AreEqual(0, wizardControl.PreviousButton.TabIndex);
            Assert.AreEqual(1, wizardControl.NextButton.TabIndex);
        }
        [Test]
        public void TestNext()
        {
            //Execute ---------------------------------------------------
            _wizardControl.Next();
            //Assert Results --------------------------------------------
            Assert.AreSame(_controller.ControlForStep2, _wizardControl.CurrentControl);
        }

        [Test]
        public void TestPrevious()
        {
            //Setup ----------------------------------------------------
            _wizardControl.Next();
            //Execute ---------------------------------------------------
            _wizardControl.Previous();
            //Assert Results --------------------------------------------
            Assert.AreSame(_controller.ControlForStep1, _wizardControl.CurrentControl);
        }

        [Test, ExpectedException(typeof(WizardStepException))]
        public void TestNextWithNoNextStep()
        {
            //Setup ----------------------------------------------------
            _wizardControl.Next();
            //Execute ---------------------------------------------------
            _wizardControl.Next();
        }

        [Test, ExpectedException(typeof(WizardStepException))]
        public void TestPreviousWithNoNextStep()
        {
            //Execute ---------------------------------------------------
            _wizardControl.Previous();
        }

        [Test]
        public void Test_Click_NextButton()
        {
            //Execute ---------------------------------------------------
            _wizardControl.NextButton.PerformClick();
            //Assert Results --------------------------------------------
            Assert.AreSame(_controller.ControlForStep2, _wizardControl.CurrentControl);
        }

        [Test]
        public void Test_ClickPreviousButton()
        {
            _wizardControl.Next();
            //Execute ---------------------------------------------------
            _wizardControl.PreviousButton.PerformClick();
            //Assert Results --------------------------------------------
            Assert.AreSame(_controller.ControlForStep1, _wizardControl.CurrentControl);
        }

        [Test]
        public void TestNextButtonText()
        {
            //Execute ---------------------------------------------------
            _wizardControl.Next();
            //Assert Results --------------------------------------------
            Assert.AreEqual("Finish", _wizardControl.NextButton.Text);
            //Execute ---------------------------------------------------
            _wizardControl.Previous();
            //Assert Results --------------------------------------------
            Assert.AreEqual("Next", _wizardControl.NextButton.Text);
        }

        [Test]
        public void TestPreviousButtonDisabledAtStart()
        {
            Assert.IsFalse(_wizardControl.PreviousButton.Enabled);
        }

        [Test]
        public void TestPreviousButtonEnabledAfterStart()
        {
            //--------------setup-----------------
            this._controller.ControlForStep2.AllowCanMoveBack = true;
            //---------------Execute-------------
            _wizardControl.Next();
            //Assert Results --------------------------------------------
            Assert.IsTrue(_wizardControl.PreviousButton.Enabled);
        }

        [Test]
        public void TestPreviousButtonDisabled_ReturnToFirstStep()
        {
            _wizardControl.Next();
            //Execute ---------------------------------------------------
            _wizardControl.Previous();
            //Assert Results --------------------------------------------
            Assert.IsFalse(_wizardControl.PreviousButton.Enabled);
        }

        [Test]
        public void Test_CallFinisheEvent()
        {
            //---------------Set up test pack-------------------
            bool finished = false;
            _wizardControl.Finished += delegate { finished = true; };
            //---------------Execute Test ----------------------
            _wizardControl.Next();
            _wizardControl.Finish();
            //---------------Test Result -----------------------
            Assert.IsTrue(_controller.FinishCalled);
            Assert.IsTrue(finished);
        }

        [Test, ExpectedException(typeof(WizardStepException))]
        public void TestFinishAtNonFinishStep()
        {
            _wizardControl.Finish();
        }

        [Test]
        public void TestNextClickAtLastStep()
        {
            //---------------Set up test pack-------------------
            _wizardControl.Next();
            //-=----------Assert preconditions ----------------------------
            Assert.IsFalse(_controller.FinishCalled);

            //Execute ---------------------------------------------------
            _wizardControl.NextButton.PerformClick();
            //---------------Assert result -----------------------------------

            Assert.IsTrue(_controller.FinishCalled);
        }

        [Test]
        public void TestNextClickAtLastStepCallsCanMoveOn()
        {
            //---------------Set up test pack-------------------
            WizardControllerStub controller = new WizardControllerStub();
            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(controller);// new WizardControl(_controller);
            controller.ControlForStep1.AllowMoveOn = true;
            controller.ControlForStep2.AllowMoveOn = false;

            wizardControl.Start();
            wizardControl.NextButton.PerformClick();
            //---------------Execute Test ----------------------
            wizardControl.NextButton.PerformClick();
            //---------------Test Result -----------------------
            // Finish should not have been called because CanMoveOn on step two is returning false and should prevent
            // the wizard from finishing.
            Assert.IsFalse(controller.FinishCalled);
        }

        [Test]
        public void TestFinishEventPosted()
        {
            //---------------Set up test pack-------------------
            _wizardControl.Next();
            bool finishEventPosted = false;
            _wizardControl.Finished += delegate { finishEventPosted = true; };
            //-=----------Assert preconditions ----------------------------
            Assert.IsFalse(finishEventPosted);
            //Execute ---------------------------------------------------
            _wizardControl.NextButton.PerformClick();
            //---------------Assert result -----------------------------------
            Assert.IsTrue(finishEventPosted);

        }
        [Test]
        public void TestNextWhen_CanMoveOn_False_TestMessagPostedEventCalled()
        {
            //---------------Setup wizard Control -------------------------------
            _wizardControl.MessagePosted += delegate(string message) { _message = message; };
            _controller.ControlForStep1.AllowMoveOn = false;
            //---------------Execute Test ------------------------------------
            _wizardControl.Next();
            //---------------Assert result -----------------------------------
            Assert.AreSame(_controller.ControlForStep1, _wizardControl.CurrentControl);
            Assert.AreEqual("Sorry, can't move on", _message);

        }

        [Test]
        public void TestPreviousButtonDisabledIfCanMoveBackFalse()
        {
            //---------------Set up test pack-------------------
            WizardControllerStub wizardController = new WizardControllerStub();
            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(wizardController);
            wizardController.ControlForStep2.AllowCanMoveBack = false;
            wizardControl.Start();

            //---------------Assert Preconditions ----------------------
            Assert.IsFalse(wizardController.ControlForStep2.CanMoveBack());           
            //---------------Execute Test ----------------------
            wizardControl.Next();
            //---------------Assert result -----------------------
            Assert.AreSame(wizardControl.CurrentControl, wizardController.ControlForStep2);
            Assert.IsFalse  (((WizardStepStub)wizardControl.CurrentControl).AllowCanMoveBack);
            Assert.IsFalse(wizardControl.PreviousButton.Enabled);
        }

        [Test]
        public void TestPreviousButtonDisabledIfCanMoveBackFalse_FromPreviousTep()
        {
            //TODO: setup with 3 steps set step 2 allow move back false
            //and go next next next previous and then ensure that canMoveBack false
            //---------------Set up test pack-------------------
            WizardControllerStub wizardController = new WizardControllerStub();
            wizardController.ForTestingAddWizardStep(new WizardStepStub());

            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(wizardController);
            wizardController.ControlForStep2.AllowCanMoveBack = false;
            wizardControl.Start();

            //---------------Assert Preconditions ----------------------
            Assert.IsFalse(wizardController.ControlForStep2.CanMoveBack());
            //---------------Execute Test ----------------------
            wizardControl.Next();
            wizardControl.Next();
            wizardControl.Previous();
            //---------------Assert result -----------------------
            Assert.AreSame(wizardControl.CurrentControl, wizardController.ControlForStep2);
            Assert.IsFalse(((WizardStepStub)wizardControl.CurrentControl).AllowCanMoveBack);
            Assert.IsFalse(wizardControl.PreviousButton.Enabled);
        }

        [Test]
        public void Test_SetStepResizesControl()
        {
            //---------------Set up test pack-------------------
            WizardControllerStub wizardController = new WizardControllerStub();
            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(wizardController);
            wizardControl.Start();
            wizardController.ControlForStep2.Width = 10;

            //--------------Assert PreConditions----------------            
            string msg;
            Assert.IsTrue(wizardController.CanMoveOn(out msg));

            //---------------Execute Test ----------------------
            wizardControl.Next();

            //---------------Test Result -----------------------
            Assert.AreEqual(wizardControl.Width- WizardControl.PADDING*2, wizardController.ControlForStep2.Width);
        }

        [Test, Ignore("The test is visually working but the tests are not picking up a change in width")]
        public void TestNextPreviousIn_theCorrectOrder()
        {
            //---------------Set up test pack-------------------
            WizardControllerStub wizardController = new WizardControllerStub();
            //--------------Assert PreConditions----------------            

            //---------------Execute Test ----------------------
            IWizardControl wizardControl = GetControlFactory().CreateWizardControl(wizardController);

            //---------------Test Result -----------------------
            Assert.Less(wizardControl.NextButton.Left, wizardControl.PreviousButton.Left);
            //---------------Tear Down -------------------------          
        }
        internal class WizardControllerStub : IWizardController
        {
            public WizardStepStub ControlForStep1 = new WizardStepStub();
            
            public WizardStepStub ControlForStep2 = new WizardStepStub("This is wizard step 2");
            public bool FinishCalled = false;
            private readonly List<IWizardStep> _wizardSteps;
            private int _currentStep = -1;
            private bool _cancelButtonEventFired = false;

            public WizardControllerStub()
            {
                _wizardSteps = new List<IWizardStep>();
                ControlForStep1.Name = "ControlForStep1";
                ControlForStep2.Name = "ControlForStep2";
                _wizardSteps.Add(ControlForStep1);
                _wizardSteps.Add(ControlForStep2);
                
            }

            public IWizardStep GetNextStep()
            {
                if (_currentStep < _wizardSteps.Count - 1)
                    return _wizardSteps[++_currentStep];
                else
                    throw new WizardStepException("Invalid Wizard Step: " + (_currentStep + 1));
            }

            public IWizardStep GetPreviousStep()
            {
                if (_currentStep > 0)
                    return _wizardSteps[--_currentStep];
                else throw new WizardStepException("Invalid Wizard Step: " + (_currentStep - 1));
            }


            public IWizardStep GetFirstStep()
            {
                FinishCalled = false;
                return _wizardSteps[_currentStep = 0];
            }

            public bool IsLastStep()
            {
                return (_currentStep == _wizardSteps.Count - 1);
            }

            public bool IsFirstStep()
            {
                return (_currentStep == 0);
            }

            public void Finish()
            {
                if (IsLastStep())
                    FinishCalled = true;
                else throw new WizardStepException("Invalid call to Finish(), not at last step");
            }

            public bool CanMoveOn(out string message)
            {
                return _wizardSteps[_currentStep].CanMoveOn(out message);
            }

            public int StepCount
            {
                get { return _wizardSteps.Count; }
            }

            /// <summary>
            /// Gets or Sets the Current Step of the Wizard.
            /// </summary>
            public int CurrentStep
            {
                get { return _currentStep; }
            }

            public bool CancelButtonEventFired
            {
                get { return _cancelButtonEventFired; }
            }

            public IWizardStep GetCurrentStep()
            {
                return _wizardSteps[_currentStep];
            }

            /// <summary>
            /// This provides a method which is called when the wizard is cancelled. The wizard controller can 
            /// undo any changes that have occured up until that point so as to ensure that the objects are returned
            /// to their original state.
            /// </summary>
            public void CancelWizard()
            {
                this._cancelButtonEventFired = true;
            }

            public void ForTestingAddWizardStep(IWizardStep step)
            {
                _wizardSteps.Add(step);
            }
        }

        internal class WizardStepStub :ControlGiz, IWizardStep
        {
            private bool _allowMoveOn = true;

            private readonly string _headerText;
            private bool _allowCanMoveBack = true;
            private bool _isInitialised;

            public string HeaderText
            {
                get { return _headerText; }
            }

            /// <summary>
            /// Provides an interface for the developer to implement functionality to cancel any edits made as part of this
            /// wizard step. The default wizard controller functionality is to call all wizard steps cancelStep methods when
            /// its Cancel method is called.
            /// </summary>
            public void CancelStep()
            {
                
            }

            public WizardStepStub() : this("")
            {
            }

            public WizardStepStub(string headerText)
            {
                _headerText = headerText;
            }

            public bool AllowCanMoveBack
            {
                get { return _allowCanMoveBack; }
                set { _allowCanMoveBack = value; }
            }

            #region IWizardStep Members

            public void InitialiseStep()
            {
                _isInitialised = true;
            }

            public bool CanMoveOn(out string message)
            {
               message = "";
               if (!AllowMoveOn) message = "Sorry, can't move on";
                return AllowMoveOn;
            }

            /// <summary>
            /// Verifies whether the user can move back from this step.
            /// </summary>
            /// <returns></returns>
            public bool CanMoveBack()
            {
                return AllowCanMoveBack;
            }

            #endregion

            public bool AllowMoveOn
            {
                get { return _allowMoveOn; }
                set { _allowMoveOn = value; }
            }

            IControlCollection IControlChilli.Controls
            {
                get
                {
                    return null;

                }
            }

            public bool IsInitialised
            {
                get { return _isInitialised; }
            }

            ///<summary>
            ///Returns a <see cref="T:System.String"></see> containing the name of the <see cref="T:System.ComponentModel.Component"></see>, if any. This method should not be overridden.
            ///</summary>
            ///
            ///<returns>
            ///A <see cref="T:System.String"></see> containing the name of the <see cref="T:System.ComponentModel.Component"></see>, if any, or null if the <see cref="T:System.ComponentModel.Component"></see> is unnamed.
            ///</returns>
            ///
            public override string ToString()
            {
                return Name;
            }
        }
    }
}
