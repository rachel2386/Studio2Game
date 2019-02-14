Favorites Tab[s] for Unity
version 1.2.19, November 2017
Copyright © 2012-2017, by Flipbook Games
--------------------------------

Your personalized list of favorite assets and scene objects


Version 1.2.19:
- New option to filter by star color
- New option to show stars in favorites tabs
- Fixed NRE when no Favorites tab is open

Version 1.2.18:
- Upgraded for compatibility with Unity 5.4

Version 1.2.17:
- Upgraded for compatibility with Unity 4.7 and Unity 5.3

Version 1.2.16:
- New option to show bookmark ribbon icons instead of stars

Version 1.2.15:
- New option to fix positions of other icons in the Hierarchy view
- New sorting option - Most recently favorited on top
- New - Add to Favorites & select star color by right-clicking a hollow star
- Bug fixes

Version 1.2.14:
- A quick update for Unity 5.1

Version 1.2.13:
- Selected favorite folder will automatically expand in a single-column Project view

Version 1.2.12:
- Fixed: Delete and Backspace in the search field were executing "Remove from Favorites"
- Fixed: Selecting another favorite item using keyboard after changing filters
- Added Enter/Return key moves focus from search box to favorites list

Version 1.2.11:
 - Added "Show in New Inspector" context menu on items in Project View
 - Fixed "Show in New Inspector" right after favoriting a scene object (thanks to Yann Papouin for finding the issue)
 - Fixed not showing hollow stars sometimes
 - Fixed rare NullReferenceException when accessing mouseOverWindow
 - Fixed keyboard shortcut shown on "Remove from Favorites" context menu

Version 1.2.10:
 - Added option to show assets' locations
 - Added "Show in New Inspector" feature
 - Delete and Backspace keys now remove selected items from favorites list

Version 1.2.9:
 - Compatible with Hierarchy2 v1.3

Version 1.2.8:
 - Compatible with Hierarchy2 (thanks to Jesse Werner's idea)

Version 1.2.7:
 - Fixed showing content of favorite folders in Unity 4.3 (thanks to Maurizio for discovering the issue)

Version 1.2.6:
 - Fixed (again) performance issue with no FG_GameObjectGUIDs (re-introduced with v1.2.5).

Version 1.2.5:
 - Fixed lost references to scene objects after entering game mode on modified scenes (thanks to Jimww).
 - Fixed stars blinking in Hierarchy view when entering game mode with a game object selected (thanks to Jimww again).

Version 1.2.4:
 - Fixed positioning of Antares Universe icons (thanks to Nezabyte).

Version 1.2.3:
- Shows the content of bookmarked folders in the second column of Project view in Unity 4.

Version 1.2.2:
- Fixed performance issues in scenes with no FG_GameObjectGUIDs (thanks to Jim_Young).

Version 1.2.1:
- Fixed title initialization on Favorites tabs hidden behind another tab.
 
New in version 1.2:
- Support for showing multiple favorites tabs, each with different filtering to show diffent sets of favorites.
- Filtering and search setting for each favorites tab are persistent between Unity sessions.
- Many new filtering options added to filter by asset type.
- Favorites tabs filtered by type show the selected type in the title.
- Star icons for each favorite item can optionally be set to colors other than the default yellow star, independently for each user.
- FG_GameObjectGUIDs game object gets created only when it is needed and can optionally be deleted if user wants that.
- A custom inspector appears on FG_GameObjectGUIDs, explaining the function of this game object.
- Added context menu items on favorite assets to reimport them and to show them in Explorer (reveal in Finder on Mac).
- Editor/Resources folder renamed to Editor/Textures to avoid inclusion of those assets in the final builds.
- Unity 4 support.

New in version 1.1:
- Multiple selected favorite items can be removed from the Favorites Tab at once.
- FG_GameObjectGUIDs game object is not hidden anymore.

Initial Release Features:
- Native look and feel, very similar to Project and Hierarchy views!
- No learning required! Just use your common Unity Editor knowledge and see it working as you would expect.
- Easy to mark or unmark favorite assets and scene object with just a single mouse-click.
- Easy to spot your favorite assets or scene objects in the Project and Hierarchy window, even when the Favorites Tab is closed!
- Favorites Tab displays all favorites sorted by name or type.
- Search by name functionality.
- Filters to show only assets or scene objects.
- Keyboard and mouse are fully supported.
- Selection synchronization. Select an item in the Favorites Tab to easily find it in the Hierarchy or Project views.
- Multiple favorite items selections.
- Dragging items from the Favorites Tab to any other Unity view is fully supported.
- Double click or press F key (or use context menu) to Frame the selected scene object in the Scene View, same as from the Hierarchy view.
- Double click or press Enter key (or use context menu) to open the selected asset, same as from the Project view.
- Works with teams! All team members have their own list of favorites even if they share the same project!
- GUID based asset references, so that assets exported and imported into another project remain in your list of favorites.
- Full source code provided for your reference or modification! :-)

Follow me on http://twitter.com/FlipbookGames
Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
Join Unity forum discusion http://forum.unity3d.com/threads/149856
Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
Visit http://flipbookgames.com/ for more info.


1. Introduction

Favorites Tab[s] is an Editor extension Unity that helps users access their most often used assets and scene objects, such as prefabs, behavior scripts, scenes, asset folders, or perhaps scene objects they are currently working on. Special care has been taken to extremely simplify adding new favorite items and removing them! These functions are always easily accessible as toggle buttons shown on Hierarchy and Project view items, even when no Favorites tab is visible. Favorite items are displayed with a star icon to help users spot them more easily. Recently an option to change the colors of stars has been added (through a context menu on the star icons).


2. Motivation

As the project grows with new assets and number of scene objects increases on a daily basis finding assets and scene objects becomes more and more difficult. As a result the development becomes slower and slower as the project advances into later stages. The Favorites Tab[s] was created in order to improve the common workflow inside Unity Editor and keep the pace of the development process fast regardless of project's and scene's size or their current stage.


3. How to use the Favorites Tab[s]

No special training is needed to start using the Favorites Tab[s]. It was designed to be used simply and easily since the beginning. Common knowledge of using the Unity Editor should be sufficient to work with the Favorites Tab[s] since its UI, functionality, and its overall look and feel follow the standard user interface elements of the Editor. The Favorites Tab[s] works and looks very similar to the standard Hierarchy and Project views, which not only helps to blend and fit nicely within Editor's environment, but it also minimizes the time spent to start using the Favorites Tab[s] efficiently.

Just in case, here's a short description of the Favorites Tab[s]' UI:

- To open a new Favorites tab choose Window->Favorites from the main menu, or View->Add Tab from the Favorites tab's toolbar. You can also use the Ctrl+T (Cmd+T) shortcut while a Favorites tab is focused.
- To add a new item click the hollow star icon displayed next to items in the Hierarchy and Project views. The icon turns into a yellow star and remains there to indicate that the item is in your list of favorites.
- To remove items from the favorites list click the yellow star icon displayed next to them in the Hierarchy and Project views, or use item's context menu inside the Favorites tab.
- To change the star colors of favorite items right-click the star icon and select the color from the context menu.
- Select items in the Favorites tabs same as you would do that in the Hierarchy and Project views, using keyboard or mouse. Multiple selections are also supported.
- Drag items from the Favorites tabs and drop them on other Editor views to perform various actions, same as you would do that from Hierarchy and Project view.
- Double-click scene object items to frame them in the Scene view, same as you would do that in the Hierarchy view.
- Double-click asset items to open them, same as you would do that in the Project view. 
- Search for items by name inside a Favorites tab, same as you would do that in the Hierarchy and Project views. Use the Ctrl+F (Cmd-F) shortcut to focus the search box and Esc to move the focus back on the list, or simply simply use the mouse for these actions.
- Activate or deactivate filtering by type from the search field's drop-down menu.
- Sort items in the Favorites Tab by name or type using the drop-down menu of the View toolbar button. Also check the About box from there ;)
- Enjoy this improved workflow and don't forget to share your feedback ;)

Useful tip: To clear your list of favorites simply select all items inside a Favorites tab with Ctrl+A (Cmd-A) then right-click and select 'Remove from Favorites' in the context menu.


4. Support, Bugs, Requests, and Feedback

Please feel free to contact Flipbook Games at info@flipbookgames.com, visit http://flipbookgames.com, and like Flipbook Games on Facebook http://www.facebook.com/FlipbookGames


Thanks for purchasing the Favorites Tab[s]!
Your feedback is very welcome!
Enjoy :)
