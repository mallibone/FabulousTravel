﻿// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace FabulousTravel.Android

open System

open Android.App
open Android.Content
open Android.Content.PM
open Android.Runtime
open Android.Views
open Android.Widget
open Android.OS
open Xamarin.Forms.Platform.Android

[<Activity (Label = "FabulousTravel.Android", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation))>]
type MainActivity() =
    inherit FormsAppCompatActivity()
    override this.OnCreate (bundle: Bundle) =
        FormsAppCompatActivity.TabLayoutResource <- Resources.Layout.Tabbar
        FormsAppCompatActivity.ToolbarResource <- Resources.Layout.Toolbar
        base.OnCreate (bundle)

        Xamarin.Essentials.Platform.Init(this, bundle)

        //Xamarin.Forms
        global.Xamarin.Forms.Forms.SetFlags([|"Shell_Experimental"; "CollectionView_Experimental"; "Visual_Experimental"|])
        Xamarin.Forms.Forms.Init (this, bundle)

        let appcore  = new FabulousTravel.App()
        this.LoadApplication (appcore)

    override this.OnRequestPermissionsResult(requestCode: int, permissions: string[], [<GeneratedEnum>] grantResults: Android.Content.PM.Permission[]) =
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults)

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults)
