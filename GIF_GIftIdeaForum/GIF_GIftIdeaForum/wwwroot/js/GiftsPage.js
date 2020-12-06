var from = 0;
var to = 15;
var globalItems;

function GenerateList(Items) {
    globalItems = Items;
    Items.forEach(element => console.log(element));

    var parent = document.getElementById("GiftContainer")
    for (var i = from; i < Items.length && i < to; i++) {
        var newDiv = document.createElement("div");

        const name = document.createTextNode(Items[i].dataBaseObject.name); 
        newDiv.appendChild(name);

        const upvotes = document.createTextNode(Items[i].dataBaseObject.upVotes);
        //upvotes.id = Items[i].dataBaseObject.name;
        newDiv.appendChild(upvotes);

        const UpvoteButton = document.createElement("button");
        UpvoteButton.innerText = "Upvote";
        UpvoteButton.addEventListener("click", UpvoteItem.bind(null, Items[i].dataBaseObject.key, i));

        const Image = document.createElement("img");

        Image.src = Items[i].uri;

        newDiv.id = Items[i].dataBaseObject.key;
        newDiv.appendChild(UpvoteButton);
        newDiv.appendChild(Image);
        parent.appendChild(newDiv);
    }
    from += 15;
    to += 15;
}

function UpvoteItem(id, index) {

    if (typeof (Storage) !== "undefined") {
        if (window.localStorage.getItem(id) == null) {
            window.localStorage.setItem(id, "voted");
            DotNet.invokeMethodAsync('GIF_GIftIdeaForum', 'CallVotes', id);
            const up = document.getElementById(id);
            var textNode = up.childNodes;
            textNode[1].nodeValue = globalItems[index].dataBaseObject.upVotes + 1;
        }
        else {
            alert("Already Upvoted Item!");
        }
    } else {
        alert("Sorry, your browser does not support Web Storage...");
    }
}