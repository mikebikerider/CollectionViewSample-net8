I wrote this sample app to explore why MAUI CollectionView performs so poorly compared to Xamarin.Forms CollectionVew or ListView.

The app has two pages that can be navigated between via the Navigation bar button. Naviagation bar can be hidden and navigation controls are moved to the bottom bar. There is a check box for that in Settings.

The first page shows a vertical scroll CollectionView with a 3-columns header and 3-columns content defined in Xaml data template.

The second page has vertical sroll 4-columns CollectionView placed inside horizontal ScrollView.

The header is not CollectionView built-in header that scrolls together with items. Mine stays on top.

The number of items initially loaded in either CollectionView is 50 in Debug mode and 100 in Release mode.

The number of items loaded after the user taps the bottom bar 'Load' button is 200 in Debug mode and 5000 in Release mode. That number can be changed in Settings individually for each CollectionView.

In Xamarin.Forms placing a CollectionViw or ListView inside a ScrollView has no side effects as long as the ScrollView scroll direction is different from CollectionView/ListView scroll direction.
Not so in MAUI. I think the cause of poor CollectionView performance is that controls placed inside MAUI ScrollView are not properly sized. Not just CollectionView or ListView.

The app bottom bar is a 3-columns 2-rows grid placed inside a horizontal scroll ScrollView inside a border. 
The bottom grid ColumnDefinitions="auto,*,auto" is holding 3 ImageButtons. The 'Settings' ImageButton in the middle star column and should be in the center of the bottom bar. The Up/Down ImageButton in the 'auto' column should be at the right edge. On iOS it works without extra code. MAUI Android ScrollView  handles the size of the 3-column grid as if the middle 'star' column is 'auto'. To achieve the desired look I override OnSizeAllocated() adding a line:

	bottomBar.WidthRequest = width in OnSizeAllocated();

The CollectionView placed inside a ScrollView requires more complex correction. 
The sample CollectionView item defined in the DataTemplate is a 1-row multiple columns grid. The last column Width is 'star'. Other columns Width is bound to the data source. The width of the columns is calculated prior to assigning the ItemsSource. In Xamarin.Forms placing a CollectionView iniside a ScrollView makes no difference on both iOS and Android. Not so in MAUI. MAUI CollectionView placed inside a ScrollView is not sized properly. It can be somewhat mitigated by placing weird platform-dependend code lines in SizeAllocated(). Not good enough for a production app. In iOS scrolling up the CollectionView that is inside a horizontal scroll ScrollView can move CollectionView parent grid up so header moves together with CollectionView content. Does not happen each time, but often enough to notice. That never happens on Android. iOS horizontal sizing is a compromise. On iOS issuing WidthRequest that is equal page Width results in CollectionView Width that is bigger than requested and grid can be scrolled horizontally in landscape when it shouldn't. If I skip WidthRequest() in SizeAllocated for iOS the items are correctly sized when device rotated to landscape, but when it is rotated back to portrait the star column width does not change back.

When the app is launched in VS Debugger on Android device or simulator the debug output shows multiple GC calls when the CollectionView is loaded or is scrolled. That happens on the first page that has no horizontal scrolling too, only it is much worse on the second page. The output also shows:

[Choreographer] Skipped 37 frames!  The application may be doing too much work on its main thread.

In Xamarin.Forms those issues do not exist. It seems to me that Xamarin.Forms renderes do much better job than MAUI handlers. In Xamarin I populate the CollectionView with data before the CollectionView is shown and it is fast. In MAUI I use activity indicator to show data is loading. Even in release mode activity indicator is necessary. In Xamarin.Forms there is no need for that. Content is loaded literally in a blink of an eye. 
 

Michael Rubinstein 
