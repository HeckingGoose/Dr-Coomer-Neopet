using SDL2Test;
using SDL2;
using System.Diagnostics;
using System;
using System.Collections;

namespace Dr_Coomer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Constants
            // Declare constants
            const int TARGETWIDTH = 400, TARGETHEIGHT = 640;
            const Byte TARGETFRAMERATE = 60;
            // Game constants
            const double TIMEBETWEENUPDATES = 2;
            const int DECAYCONSTANT = 1;
            const int MAXHUNGER = 100,
                MAXTHIRST = 100,
                MAXHAPPINESS = 100;
            const double MOUTHSWITCHFREQUENCY = 0.2;
            #endregion

            // Initialise window and renderer, then store pointer results
            (IntPtr window, IntPtr renderer) o = SDL_HFC.Init("Dr Coomer ", TARGETWIDTH, TARGETHEIGHT);

            #region Variables
            // Create variables
            // Pointer trackers
            List<IntPtr> textures = new List<IntPtr>();
            List<IntPtr> audioPointers = new List<IntPtr>();
            Dictionary<int, IntPtr> fonts = new Dictionary<int, IntPtr>();

            // Audio playing timer, since callback functions don't work in this wrapper
            double playingTimer = 0;

            // Keyboard tracking
            Dictionary<string, bool> keyMap = new Dictionary<string, bool>()
            {
                { "escape", false },
            };

            // Colours
            SDL.SDL_Color white = new SDL.SDL_Color();
            white.r = 255;
            white.g = 255;
            white.b = 255;
            white.a = 255;

            // Game variables
            Byte hunger = MAXHUNGER, thirst = MAXTHIRST, happiness = MAXHAPPINESS;
            UInt32 playcoins = 0;
            double passedTime = 0;

            // Mouse
            int x = 0, y = 0;
            bool mouseHeld = false;

            // Game state
            // 0: Main screen
            // 1: Shop
            int state = 0;

            // Coomer state
            // false: mouth closed
            // true: mouth open
            bool coomerState = false;
            double coomerStateTimer = 0;
            bool statedHunger = false,
                statedThirst = false,
                statedHappiness = false;
            #endregion

            #region Asset loading
            // LOAD FONTS
            fonts = SDL_HFC.LoadFonts(@"Fonts\m5x7.ttf", new int[2] { 48, 60 });
            // END LOAD FONTS

            // CREATE ICON
            // Load icon
            Image icon = SDL_HFC.LoadImage(@"Images\icon.png");
            textures.Add(icon.Pointer);
            // Set display icon
            SDL.SDL_SetWindowIcon(o.window, icon.Pointer);
            // END CREATE ICON

            // LOAD TEXTURES
            // Create textures
            Texture coomerClosed = SDL_HFC.LoadTexture(o.renderer, @"Images\coomerclosed.png");
            Texture coomerOpen = SDL_HFC.LoadTexture(o.renderer, @"Images\coomerOpen.png");
            Texture pistol = SDL_HFC.LoadTexture(o.renderer, @"Images\pistol.png");
            Texture pizza = SDL_HFC.LoadTexture(o.renderer, @"Images\pizza.png");
            Texture soda = SDL_HFC.LoadTexture(o.renderer, @"Images\soda.png");

            // Track pointers
            textures.Add(coomerClosed.Pointer);
            textures.Add(coomerOpen.Pointer);
            textures.Add(pistol.Pointer);
            textures.Add(pizza.Pointer);
            textures.Add(soda.Pointer);
            // END LOAD TEXTURES

            // LOAD AUDIO
            // Load WAV files
            WAV helloGordan = new WAV(@"Audio\hellogordan.wav");
            WAV imThirsty = new WAV(@"Audio\imthirsty.wav");

            // Store pointers
            audioPointers.Add(helloGordan.Pointer);
            audioPointers.Add(imThirsty.Pointer);

            // Create audio channel
            uint deviceID = SDL_HFC.CreateAudioChannel(helloGordan.AudioSpec);

            // Unpause channel
            SDL.SDL_PauseAudioDevice(deviceID, 0);
            // END LOAD AUDIO

            // GENERATE TEXT SURFACES
            // Create surfaces
            Texture shopText = SDL_HFC.TextureFromText(o.renderer, "Shop", fonts[48], white);
            Texture buyText = SDL_HFC.TextureFromText(o.renderer, "Buy", fonts[48], white);

            Texture sodaText = SDL_HFC.TextureFromText(o.renderer, "Soda", fonts[60], white);
            Texture pizzaText = SDL_HFC.TextureFromText(o.renderer, "Pizza", fonts[60], white);
            Texture killText = SDL_HFC.TextureFromText(o.renderer, "Kill a man", fonts[60], white);

            Texture thirstText = SDL_HFC.TextureFromText(o.renderer, "+15 Thirst", fonts[48], white);
            Texture hungerText = SDL_HFC.TextureFromText(o.renderer, "+15 Hunger", fonts[48], white);
            Texture happyText = SDL_HFC.TextureFromText(o.renderer, "+15 Happiness", fonts[48], white);

            Texture costText = SDL_HFC.TextureFromText(o.renderer, "-5 Playcoins", fonts[48], white);

            // Track pointers
            textures.Add(shopText.Pointer);
            textures.Add(buyText.Pointer);

            textures.Add(sodaText.Pointer);
            textures.Add(pizzaText.Pointer);
            textures.Add(killText.Pointer);

            textures.Add(thirstText.Pointer);
            textures.Add(hungerText.Pointer);
            textures.Add(happyText.Pointer);

            textures.Add(costText.Pointer);
            // END GENERATE TEXT SURFACES

            // CREATE BUTTONS
            List<Button_Colour> mainButtons = new List<Button_Colour>();
            List<Button_Colour> shopButtons = new List<Button_Colour>();

            SDL.SDL_Rect shopButtonRect = new SDL.SDL_Rect();
            shopButtonRect.x = 100;
            shopButtonRect.y = 550;
            shopButtonRect.w = 200;
            shopButtonRect.h = 50;

            SDL.SDL_Rect buyPizzaRect = new SDL.SDL_Rect();
            buyPizzaRect.x = 50;
            buyPizzaRect.y = 180;
            buyPizzaRect.w = 100;
            buyPizzaRect.h = 40;
            SDL.SDL_Rect buySodaRect = new SDL.SDL_Rect();
            buySodaRect.x = 50;
            buySodaRect.y = 380;
            buySodaRect.w = 100;
            buySodaRect.h = 40;
            SDL.SDL_Rect buyKillRect = new SDL.SDL_Rect();
            buyKillRect.x = 50;
            buyKillRect.y = 580;
            buyKillRect.w = 100;
            buyKillRect.h = 40;
            SDL.SDL_Rect backButtonRect = new SDL.SDL_Rect();
            backButtonRect.x = 290;
            backButtonRect.y = 10;
            backButtonRect.w = 100;
            backButtonRect.h = 50;

            Button_Colour shopButton = new Button_Colour(
                0,
                o.renderer,
                "Shop",
                fonts[48],
                shopButtonRect,
                (0, 0, 0, 255),
                (255, 255, 255, 255)
                );
            Button_Colour buyPizzaButton = new Button_Colour(
                1,
                o.renderer,
                "Buy",
                fonts[48],
                buyPizzaRect,
                (0, 0, 0, 255),
                (255, 255, 255, 255)
                );
            Button_Colour buySodaButton = new Button_Colour(
                2,
                o.renderer,
                "Buy",
                fonts[48],
                buySodaRect,
                (0, 0, 0, 255),
                (255, 255, 255, 255)
                );
            Button_Colour buyKillButton = new Button_Colour(
                3,
                o.renderer,
                "Buy",
                fonts[48],
                buyKillRect,
                (0, 0, 0, 255),
                (255, 255, 255, 255)
                );
            Button_Colour backButton = new Button_Colour(
                4,
                o.renderer,
                "Back",
                fonts[48],
                backButtonRect,
                (0, 0, 0, 255),
                (255, 255, 255, 255)
                );
            mainButtons.Add(shopButton);
            shopButtons.Add(buyPizzaButton);
            shopButtons.Add(buySodaButton);
            shopButtons.Add(buyKillButton);
            shopButtons.Add(backButton);

            // Cache textures
            textures.Add(shopButton.ButtonTextTexture.Pointer);
            textures.Add(buyPizzaButton.ButtonTextTexture.Pointer);
            textures.Add(buySodaButton.ButtonTextTexture.Pointer);
            textures.Add(buyKillButton.ButtonTextTexture.Pointer);

            // END CREATE BUTTONS
            #endregion

            // Create boolean to track when program is exitted
            bool ended = false;

            // Assume first frame rendered at full speed
            float deltaTime = 1 / TARGETFRAMERATE;
            float gameSpeed = 1;

            // DO STUFF

            playingTimer = helloGordan.SecondsLength;
            SDL.SDL_QueueAudio(deviceID, helloGordan.Buffer, helloGordan.Length);

            while (!ended)
            {
                // Note frame start time
                // Ticks is in 100s of nanoseconds, so to convert to seconds, divide by 1000000
                ulong frameStartTime = SDL.SDL_GetPerformanceCounter();

                // Refresh the display
                SDL_HFC.DisplayRefresh(o.renderer, (0, 0, 0, 255));

                // Fetch mouse info
                UInt32 mouseButtonMask = SDL.SDL_GetMouseState(out x, out y);

                // Poll events and do events until all events are done
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            ended = true;
                            break;
                    }
                }
                // GAME LOGIC
                // Update timers
                passedTime += deltaTime;
                if (passedTime > TIMEBETWEENUPDATES)
                {
                    passedTime = 0;
                    happiness -= DECAYCONSTANT;
                    thirst -= DECAYCONSTANT;
                    hunger -= DECAYCONSTANT;
                }
                if (playingTimer > 0)
                {
                    playingTimer -= deltaTime;

                    // Handle mouth state
                    coomerStateTimer += deltaTime;
                    if (coomerStateTimer > MOUTHSWITCHFREQUENCY) // If it is time to invert mouth
                    {
                        coomerStateTimer = 0;
                        coomerState = !coomerState;
                    }
                }
                else
                {
                    // Reset timer to 0
                    playingTimer = 0;

                    // Close mouth
                    if (coomerState == true) // Mouth is open
                    {
                        coomerState = false;
                        coomerStateTimer = 0;
                    }
                }
                // Test for if audio alert needs playing
                if (hunger < 21)
                {
                    if (!statedHunger)
                    {
                        statedHunger = true;
                        // Play hungry SFX
                    }
                }
                else
                {
                    statedHunger = false;
                }
                if (thirst < 21)
                {
                    if (!statedThirst)
                    {
                        statedThirst = true;
                        SDL.SDL_QueueAudio(deviceID, imThirsty.Buffer, imThirsty.Length);
                        playingTimer += imThirsty.SecondsLength;
                    }
                }
                else
                {
                    statedThirst = false;
                }
                if (happiness < 21)
                {
                    if (!statedHappiness)
                    {
                        statedHappiness = true;
                        // Play unhappy SFX
                    }
                }
                else
                {
                    statedHappiness = false;
                }

                // Test losing conditions
                #region Lose conditions
                if (happiness < 1)
                {
                    // End game
                    SDL.SDL_ShowSimpleMessageBox(0,
                        "Game Over!",
                        "Dr Coomer is too sad to live :( \n" +
                        "Next time you should try to care for his happiness more.",
                        o.window);
                    ended = true;
                }
                if (thirst < 1)
                {
                    // End game
                    SDL.SDL_ShowSimpleMessageBox(0,
                        "Game Over!",
                        "Dr Coomer is too thirsty to live :( \n" +
                        "Next time you should try to care for his thirst more.",
                        o.window);
                    ended = true;
                }
                if (hunger < 1)
                {
                    // End game
                    SDL.SDL_ShowSimpleMessageBox(0,
                        "Game Over!",
                        "Dr Coomer is too hungry to live :( \n" +
                        "Next time you should try to care for his hunger more.",
                        o.window);
                    ended = true;
                }
                #endregion

                // Run logic specific to a screen
                switch (state)
                {
                    default: // Main screen
                        // Increase playcoin total if left mouse is clicked
                        if (SDL.SDL_BUTTON_LEFT == mouseButtonMask && !mouseHeld)
                        {
                            playcoins++;
                        }
                        // Update button colours
                        foreach (Button_Colour button in mainButtons)
                        {
                            if (SDL_HFC.TestBoundingBox(button.Rect, x, y)) // Mouse lies in button
                            {
                                if (SDL.SDL_BUTTON_LEFT == mouseButtonMask && !mouseHeld) // Only mouse 1 is held
                                {
                                    button.Pressed = 2;
                                    switch (button.ID)
                                    {
                                        case 0: // Shop button
                                            state = 1;
                                            break;
                                    }
                                }
                                else
                                {
                                    button.Pressed = 1;
                                }
                            }
                            else // Mouse does not lie in button
                            {
                                button.Pressed = 0;
                            }
                        }
                        break;
                    case 1: // Shop
                        // Update button colours
                        foreach (Button_Colour button in shopButtons)
                        {
                            if (SDL_HFC.TestBoundingBox(button.Rect, x, y)) // Mouse lies in button
                            {
                                if (SDL.SDL_BUTTON_LEFT == mouseButtonMask && !mouseHeld) // Only mouse 1 is held
                                {
                                    button.Pressed = 2;
                                    switch (button.ID)
                                    {
                                        case 1: // Buy pizza button
                                            if (playcoins > 4)
                                            {
                                                hunger += 15;
                                                playcoins -= 5;
                                                if (hunger > MAXHUNGER)
                                                {
                                                    hunger = MAXHUNGER;
                                                }
                                            }
                                            break;
                                        case 2: // Buy soda button
                                            if (playcoins > 4)
                                            {
                                                thirst += 15;
                                                playcoins -= 5;
                                                if (thirst > MAXTHIRST)
                                                {
                                                    thirst = MAXTHIRST;
                                                }
                                            }
                                            break;
                                        case 3: // Buy kill button
                                            if (playcoins > 4)
                                            {
                                                happiness += 15;
                                                playcoins -= 5;
                                                if (happiness > MAXHAPPINESS)
                                                {
                                                    happiness = MAXHAPPINESS;
                                                }
                                            }
                                            break;
                                        case 4: // Back button
                                            state = 0;
                                            break;
                                    }
                                }
                                else
                                {
                                    button.Pressed = 1;
                                }
                            }
                            else // Mouse does not lie in button
                            {
                                button.Pressed = 0;
                            }
                        }
                        break;
                }
                if (SDL.SDL_BUTTON_LEFT == mouseButtonMask)
                {
                    mouseHeld = true;
                }
                else
                {
                    mouseHeld = false;
                }
                // END GAME LOGIC

                    // SCREENWRITING
                switch (state)
                {
                    default: // Main screen
                        // Generate stats text
                        Texture hungerTexture = SDL_HFC.TextureFromText(
                            o.renderer,
                            $"Hunger: {hunger}",
                            fonts[48],
                            white
                            );
                        Texture thirstTexture = SDL_HFC.TextureFromText(
                            o.renderer,
                            $"Thirst: {thirst}",
                            fonts[48],
                            white
                            );
                        Texture happinessTexture = SDL_HFC.TextureFromText(
                            o.renderer,
                            $"Happiness: {happiness}",
                            fonts[48],
                            white
                            );
                        Texture playcoinsTexture = SDL_HFC.TextureFromText(
                            o.renderer,
                            $"Playcoins: {playcoins}",
                            fonts[48],
                            white
                            );

                        // Draw stats text
                        SDL.SDL_GetWindowSize(o.window, out int windowW, out int windowH);

                        SDL_HFC.DrawTexture(
                            o.renderer,
                            hungerTexture,
                            (windowW - hungerTexture.Rect.w) /2,
                            25
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            thirstTexture,
                            (windowW - thirstTexture.Rect.w) / 2,
                            75
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            happinessTexture,
                            (windowW - happinessTexture.Rect.w) / 2,
                            125
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            playcoinsTexture,
                            (windowW - playcoinsTexture.Rect.w) /2,
                            175
                            );

                        // Draw coomer
                        switch (coomerState)
                        {
                            case false: // Mouth closed
                                SDL_HFC.DrawTexture(o.renderer, coomerClosed, 50, 240, 300, 300);
                                break;
                            case true: // Mouth open
                                SDL_HFC.DrawTexture(o.renderer, coomerOpen, 50, 240, 300, 300);
                                break;
                        }
                        SDL_HFC.DrawButton_Colour(o.renderer, shopButton);

                        // Release newly created textures
                        SDL.SDL_DestroyTexture(hungerTexture.Pointer);
                        SDL.SDL_DestroyTexture(thirstTexture.Pointer);
                        SDL.SDL_DestroyTexture(happinessTexture.Pointer);
                        SDL.SDL_DestroyTexture(playcoinsTexture.Pointer);
                        break;
                    case 1: // Shop
                        // Draw shop title
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            shopText,
                            20,
                            20
                            );
                        #region Draw pizza
                        // Draw pizza and pizzaText
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            pizza,
                            30,
                            50
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            pizzaText,
                            25 + pizza.Rect.w,
                            50
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            hungerText,
                            25 + pizza.Rect.w,
                            50 + pizzaText.Rect.h
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            costText,
                            25 + pizza.Rect.w,
                            50 + pizzaText.Rect.h + hungerText.Rect.h
                            );
                        #endregion
                        #region Draw soda
                        // Draw soda and sodaText
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            soda,
                            30,
                            225
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            sodaText,
                            25 + soda.Rect.w,
                            225
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            thirstText,
                            25 + soda.Rect.w,
                            225 + sodaText.Rect.h
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            costText,
                            25 + soda.Rect.w,
                            225 + sodaText.Rect.h + thirstText.Rect.h
                            );
                        #endregion
                        #region Draw kill
                        // Draw kill and killText
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            pistol,
                            30,
                            425
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            killText,
                            25 + pistol.Rect.w,
                            425
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            happyText,
                            25 + pistol.Rect.w,
                            425 + killText.Rect.h
                            );
                        SDL_HFC.DrawTexture(
                            o.renderer,
                            costText,
                            25 + soda.Rect.w,
                            425 + killText.Rect.h + happyText.Rect.h
                            );
                        #endregion
                        // Draw buttons
                        foreach (Button_Colour button in shopButtons)
                        {
                            SDL_HFC.DrawButton_Colour(
                                o.renderer,
                                button
                                );
                        }
                        break;
                }
                // END SCREENWRITING

                // POST SCREENWRITE LOGIC

                // END SCREENWRITE LOGIC

                // Switches out the currently presented render surface with the one we just did work on.
                SDL.SDL_RenderPresent(o.renderer);

                // Update gameSpeed
                gameSpeed = deltaTime * TARGETFRAMERATE;

                // Ensure deltaTime is last calculation for accuracy
                deltaTime = (SDL.SDL_GetPerformanceCounter() - frameStartTime) / (float)SDL.SDL_GetPerformanceFrequency();
            }
            
            // END STUFF

            SDL_HFC.Cleanup(o.window, o.renderer, textures, audioPointers, fonts);
        }
    }
}