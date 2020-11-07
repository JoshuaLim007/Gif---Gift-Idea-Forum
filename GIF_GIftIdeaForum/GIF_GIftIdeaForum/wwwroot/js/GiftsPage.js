var from = 0;
var to = 5;

function GenerateList(Items) {
    Items.forEach(element => console.log(element));

    var parent = document.getElementById("GiftContainer")
    for (var i = from; i < Items.length && i < to; i++) {
        var newDiv = document.createElement("div");
        const newContent = document.createTextNode(Items[i].dataBaseObject.upVotes +" "+ Items[i].dataBaseObject.name); 
        newDiv.appendChild(newContent);
        newDiv.id = Items[i].dataBaseObject.key;
        parent.appendChild(newDiv);
    }
    from += 5;
    to += 5;
}