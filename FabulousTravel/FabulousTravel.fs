// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace FabulousTravel

open System.Diagnostics
open System
open Fabulous
open Fabulous.XamarinForms.LiveUpdate
open Fabulous.XamarinForms
open Xamarin.Forms

module App = 
    type City =
        { Name : string
          Country : string
          Image : string
          Rating : decimal
          IsFavorite : bool }

    type Model = 
      { CurrentCity : City
        Cities : City list }

    type Msg = 
        | City of City 
        | ToggleFavorite of City 

    let zurich = { Name = "Zurich"; Image = "Zurich"; Country = "Switzerland"; Rating = 4.5m; IsFavorite = true}
    let london = { Name = "London"; Image = "London"; Country = "UK"; Rating = 4.8m; IsFavorite = false}
    let seattle = { Name = "Seattle"; Image = "Seattle"; Country = "USA"; Rating = 5m; IsFavorite = false}

    let initModel = { CurrentCity = zurich; Cities = [zurich; london; seattle] }

    let init () = initModel, Cmd.none

    let update msg model =
        match msg with
        | City selectedCity -> { model with CurrentCity = selectedCity }, Cmd.none
        | ToggleFavorite city -> { model with CurrentCity = {city with IsFavorite = not city.IsFavorite } }, Cmd.none

    let magnify = "\uf349"
    let star = "\uf4ce"
    let heartFilled = "\uf2d1"
    let heartOutline = "\uf2d5"

    let cornerRadius = 22.

    let titleFontSize = 20.
    let cardTitleFontSize = 16.
    let descriptionFontSize = 14.

    let textColor = Color.Black
    let secondaryTextColor = Color.FromHex "FFB5B5B5"
    let backgroundColor = Color.White
    let starColor = Color.FromHex "FFFFBF00"
    let favoriteColor = Color.FromHex "FFFAACC1"

    let materialFont =
        (match Device.RuntimePlatform with
                                 | Device.iOS -> "Material Design Icons"
                                 | Device.Android -> "materialdesignicons-webfont.ttf#Material Design Icons"
                                 | _ -> null)

    let titleLabel text fontSize =
        View.Label(text = text,
            fontSize = fontSize,
            textColor = textColor,
            verticalOptions = LayoutOptions.Center,
            fontAttributes = FontAttributes.Bold)

    let materialButton materialIcon backgroundColor textColor command =
        View.Button(text = materialIcon,
            command = command,
            fontFamily = materialFont,
            fontSize = 20.,
            backgroundColor = backgroundColor,
            widthRequest = 42.,
            textColor = textColor)

    let materialIcon materialIcon color =
        View.Label(text = materialIcon,
            textColor = color,
            fontFamily = materialFont,
            fontSize = 18.,
            verticalOptions = LayoutOptions.Center,
            fontAttributes = FontAttributes.Bold)

    let ratingStar percentage =
        let star = materialIcon star starColor
        let boxViewWidth = 16. - (16. * percentage)
        View.Grid(
            padding = 0.,
            margin = Thickness(0.,-4.,0.,0.),
            children = [
                star
                View.BoxView(color = backgroundColor, 
                    widthRequest = boxViewWidth,
                    horizontalOptions = LayoutOptions.End)
                ])

    let ratingControl (rating:decimal) =
        let fullNumber = Math.Ceiling(rating)
        let fraction = (rating - Math.Truncate(rating))
        View.StackLayout(orientation = StackOrientation.Horizontal,
            children = [
                for i in 1m .. fullNumber -> if i = fullNumber && fraction <> 0m then ratingStar (float fraction) else ratingStar 1.
            ])

    let favoriteIcon city dispatch =
        let icon = if city.IsFavorite then heartFilled else heartOutline
        (materialButton icon Color.Transparent favoriteColor (fun () -> dispatch (ToggleFavorite city)))
        |> fun(button) -> button.HorizontalOptions LayoutOptions.End
        |> fun(button) -> button.Margin (Thickness(0.,-8.,-8.,0.))
        |> fun(button) -> button.Padding 0.
        |> fun(button) -> button.HeightRequest 8.
        |> fun(button) -> button.FontSize 32.

    let titleAndDescription title titleFontSize description descriptionFontSize =
        View.StackLayout(
            margin = 0.,
            children=[
            titleLabel title titleFontSize
            View.Label(text = description,
                margin = Thickness(0.,-10.,0.,0.),
                textColor = secondaryTextColor,
                fontSize = descriptionFontSize
                )]
            )

    let cityDescriptionFrame city dispatch =
        View.StackLayout(
            margin = Thickness(16.,0.,16.,0.),
            children = [
                View.Frame(
                    heightRequest = 320.,
                    cornerRadius = cornerRadius,
                    padding = 0.,
                    isClippedToBounds = true,
                    hasShadow = true,
                    content = View.Image(
                        source = city.Image,
                        aspect = Aspect.AspectFill)
                    )
                View.Frame(
                    heightRequest = 70.,
                    margin = Thickness(24.,-64.,24.,0.),
                    padding = Thickness(20.,12.,16.,12.),
                    backgroundColor = Color.White,
                    cornerRadius = cornerRadius,
                    content = View.Grid(
                        rowdefs=["auto"; "auto" ],
                        coldefs=["*";"auto"],
                        children=[
                            (titleAndDescription city.Name titleFontSize city.Country descriptionFontSize)
                            (favoriteIcon city dispatch).GridColumn(2)
                            (ratingControl city.Rating).GridRow(1).GridColumnSpan(2)
                            ]
                    ),
                    hasShadow = true)
            ])

    let activityFrame title description image =
        View.Frame(
            margin = Thickness(16.,0.,16.,8.),
            backgroundColor = Color.White,
            cornerRadius = cornerRadius,
            content = View.Grid(
                coldefs=["auto";"*"],
                children=[
                    View.Frame(
                        heightRequest = 56.,
                        widthRequest = 64.,
                        padding = 0.,
                        isClippedToBounds = true,
                        cornerRadius = 8.,
                        margin = Thickness(-8., 0.,8.,0.),
                        content = View.Image(source = image,
                            widthRequest = 32.,
                            heightRequest = 32.,
                            aspect = Aspect.Fill)
                    )
                    (titleAndDescription title cardTitleFontSize description descriptionFontSize).GridColumn(1)
                    ]
            ),
            hasShadow = true)

    let thingsTodo city margin =
        View.StackLayout(
            orientation = StackOrientation.Vertical,
            margin = margin,
            children = [
                (titleLabel "Things to do" titleFontSize).GridRow(0)
                (activityFrame "Classic landmarks" "Classic landmarks you need to see. Check these of your bucketlist!" "ZurichLandmark").GridRow(1)
                (activityFrame "Interesting sights" "Amazing sights that will make for stunning photos!" "Sight").GridRow(2)
                (activityFrame "Restaurants" "Where to go when you're feeling peckish and want to try out the local food." "Restaurant").GridRow(3)
                ]
        )

    let view (model: Model) dispatch =
        View.ContentPage(
            backgroundColor = backgroundColor,
            content = View.ScrollView(
                content = View.Grid(
                rowdefs = ["auto"; "auto"; "auto"; "*"],
                margin = Thickness(16.,8.,16.,-6.),
                children = [
                    View.Grid(
                        coldefs = ["*"; "auto"],
                        children = [
                            (titleLabel "Destinations" titleFontSize).GridColumn(0)
                            (materialButton magnify backgroundColor secondaryTextColor (fun() -> ())).GridColumn(1)
                        ])
                    (cityDescriptionFrame model.CurrentCity dispatch).GridRow(1)
                    // todo: fix carousel view
                    //View.CarouselView(
                    //    items = [
                    //        cityDescriptionFrame
                    //        cityDescriptionFrame
                    //        cityDescriptionFrame
                    //        ]).GridRow(1)
                    (thingsTodo model.CurrentCity (Thickness(0., 32., 0., 0.))).GridRow(2)
                ]
                ))
        )

    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> XamarinFormsProgram.run app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/tools.html for further  instructions.
    //
    do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/models.html for further  instructions.
#if APPSAVE
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex -> 
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif

