This asset makes picker like IOS on uGUI.
Thank you very much for your purchase.
When there are a problem or request, please inform "brownie19870327@gmail.com".

----------------------------------------------------------------------------
How to add picker

-Added from menu.
[GameObject]->[UI]->[Picker]-> any picker

----------------------------------------------------------------------------
How to add item

-from Editor or script
1) GameObject is added to the child PickerLayoutGroup.
2) PickerItem component is add to 1)GameObject.

-from Editor
Items of existence is duplicated(Ctrl+D,CMD+D).

-from script to Picker(StringPicker,ImagePicker)
Call to Picker.AddItem().

----------------------------------------------------------------------------
How to remove item

-from Editor or script
GameObject of an item is destroyed.

-from script to Picker(StringPicker,ImagePicker)
Call to Picker.RemoveItem().

----------------------------------------------------------------------------
How to add column

-from Editor or script
1) New GameObject is created. PickerScrollRect component is add.
2) New GameObject for scrolling is created. PickerLayoutGroup component is add.
When ZoomPicker, not PickerLayoutGroup, but ZoomPickerLayoutGroup is added.
3) 1)PickerScrollRect is attached to 2)PickerLayoutGroup.ScrollRect.
4) 2)GameObject is added to the child of 1)GameObject.

-from Editor
Columns of existence is duplicated(Ctrl+D,CMD+D).

-from Editor to Picker(StringPicker,ImagePicker)
change the value of the ItemList.size from an inspector.

-from scirpt to Picker(StringPicker,ImagePicker)
call to Picker.SetColumns(int)

----------------------------------------------------------------------------
How to get selected item

-from event
receive OnSelectItem event of PickerScrollRect component.

-from script
call to PickerScrollRect.GetSelectedItem() or PickerScrollRect.GetSelectedPickerItem().

-from script to Picker
call to Picker.GetSelectedItem() or Picker.GetSelectedItems().



----------------------------------------------------------------------------
Image Licence

2600 Flag Icon Set
---------------------------
Copyright (c) 2013 Go Squared Ltd. http://www.gosquared.com/

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
---------------------------

Ninja icon:
http://kage-design.com/wp/?p=488
