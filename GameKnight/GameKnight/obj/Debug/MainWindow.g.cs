﻿#pragma checksum "..\..\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "44010611BE2CB88065FA4D1C32B1871BF788C858"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GameKnight;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace GameKnight {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button NewGame_btn;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button NewUser_btn;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CheckOwnership_btn;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox GameKnightRole_box;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CoordinatorRole_box;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox IncludeEveryone_chbx;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox UseBallot_chbx;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox BallotNum_cmbx;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox IgnoreList_box;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup MyPopup;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image GK_Standing_img;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image GK_Title_img;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/GameKnight;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 1 "..\..\MainWindow.xaml"
            ((GameKnight.MainWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.NewGame_btn = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\MainWindow.xaml"
            this.NewGame_btn.Click += new System.Windows.RoutedEventHandler(this.AddNewGame);
            
            #line default
            #line hidden
            return;
            case 3:
            this.NewUser_btn = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\MainWindow.xaml"
            this.NewUser_btn.Click += new System.Windows.RoutedEventHandler(this.AddNewUser);
            
            #line default
            #line hidden
            return;
            case 4:
            this.CheckOwnership_btn = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\MainWindow.xaml"
            this.CheckOwnership_btn.Click += new System.Windows.RoutedEventHandler(this.CheckGame);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 26 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Refresh_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.GameKnightRole_box = ((System.Windows.Controls.TextBox)(target));
            
            #line 35 "..\..\MainWindow.xaml"
            this.GameKnightRole_box.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.GameKnightRole_box_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.CoordinatorRole_box = ((System.Windows.Controls.TextBox)(target));
            
            #line 37 "..\..\MainWindow.xaml"
            this.CoordinatorRole_box.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.CoordinatorRole_box_TextChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.IncludeEveryone_chbx = ((System.Windows.Controls.CheckBox)(target));
            
            #line 40 "..\..\MainWindow.xaml"
            this.IncludeEveryone_chbx.Checked += new System.Windows.RoutedEventHandler(this.IncludeEveryone_Checked);
            
            #line default
            #line hidden
            
            #line 40 "..\..\MainWindow.xaml"
            this.IncludeEveryone_chbx.Unchecked += new System.Windows.RoutedEventHandler(this.IncludeEveryone_Checked);
            
            #line default
            #line hidden
            return;
            case 9:
            this.UseBallot_chbx = ((System.Windows.Controls.CheckBox)(target));
            
            #line 42 "..\..\MainWindow.xaml"
            this.UseBallot_chbx.Checked += new System.Windows.RoutedEventHandler(this.UseBallot_chbx_Checked);
            
            #line default
            #line hidden
            
            #line 42 "..\..\MainWindow.xaml"
            this.UseBallot_chbx.Unchecked += new System.Windows.RoutedEventHandler(this.UseBallot_chbx_Checked);
            
            #line default
            #line hidden
            return;
            case 10:
            this.BallotNum_cmbx = ((System.Windows.Controls.ComboBox)(target));
            
            #line 44 "..\..\MainWindow.xaml"
            this.BallotNum_cmbx.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.BallotNumber_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 11:
            this.IgnoreList_box = ((System.Windows.Controls.ListBox)(target));
            return;
            case 12:
            
            #line 55 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddIgnore_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 61 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RemoveIgnore_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 75 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LetsPlay);
            
            #line default
            #line hidden
            return;
            case 15:
            this.MyPopup = ((System.Windows.Controls.Primitives.Popup)(target));
            return;
            case 16:
            this.GK_Standing_img = ((System.Windows.Controls.Image)(target));
            return;
            case 17:
            this.GK_Title_img = ((System.Windows.Controls.Image)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

