Unity作業二

sxf-F15261011

專案截圖：
 <img width="865" height="521" alt="image" src="https://github.com/user-attachments/assets/9592fb8e-0781-4b55-b56f-47c9a0072863" />

Github鏈接（branch:trask2）：
https://github.com/oozeww/115VR-HW1-F15261011-sxf
Youtube鏈接：
https://www.youtube.com/watch?v=FcaOhpKqjT0&list=PLZA8CS7QLwVA&index=2
製作流程：
1.	我添加了背景、一個控制的cube、一個長方形作爲地面™，分別把他們的order in layer設置成0、1、1；給cube增加了sprite renderer可以改變樣式、增加rigidbody 2d並勾選gravity，給cube和地面同時增加了box collider 2d，運行游戲后cube可以隨重力落在地面上
2.	Assets中書寫C#脚本並導入到cube的script中，使用vector和陣列，WASD和空格鍵控制cube，實現了左右移動和跳躍
3.	我發現cube可以走出畫面，於是添加了“空氣墻”，hierarchy新建空物體，新增component“Box Collider 2D”，放置在防止cube走出畫面的地方
4.	爲了方便判斷跳躍的機會，增加了Tag：Ground，把可以恢復跳躍的地方設置為Ground，可以一勞永逸的使用
5.	我使用了我喜歡的游戲celeste的游戲素材豐富了畫面，設計了一個紅旗終點，並用四張紅旗圖片創建了一個紅旗終點小動畫；修改script：反向運動時人物圖片反轉顯示
6.	我想實現到達終點觸發慶祝動畫，在ai的幫助下使用animator視窗和書寫script成功實現，添加了背景音樂和到達終點的慶祝音效（來自https://freesound.org）
7.	我繼續完善：參考了youtuber對celeste游戲人物的控制分析，修改script、讓角色的控制手感保持舒適（運動開始幀率和結束幀率的控制、人物的gravity參數、空格跳躍高度和按壓時間相關），添加了角色跳躍在地面上的塵土和死亡判斷
