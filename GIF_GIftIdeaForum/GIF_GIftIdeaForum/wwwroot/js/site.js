// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


/*
https://www.geeksforgeeks.org/html-dom-button-object/#:~:text=Creating%20button%20object%3A%20The%20button,as%20div)%20to%20display%20it.
function Geeks() {
    var myDiv = document.getElementById("GFG");

    // creating button element
    var button = document.createElement('BUTTON');

    // creating text to be
    //displayed on button
    var text = document.createTextNode("Button");

    // appending text to button
    button.appendChild(text);

    // appending button to div
    myDiv.appendChild(button); ;
}
 */
var modal = document.getElementById("modalWindow");

// Get the button that opens the modal
var btn = document.getElementById("modalBtn");

// When the user clicks the button, open the modal 
btn.onclick = function () {
    modal.style.display = "block";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}
/*
var modal = document.getElementById("modalWindow");

var btn = document.getElementById("modalBtn");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("modla__closebtn")[0];

// When the user clicks the button, open the modal
btn.onclick = function () {
    modal.style.display = "block";
}

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}
 */

const btnHamburger = document.querySelector('#btnHamburger');
const body = document.querySelector('body');
const header = document.querySelector('.header');
const overlay = document.querySelector('.overlay');
const fadeElems = document.querySelectorAll('.has-fade');

btnHamburger.addEventListener('click', function () {
    console.log('click hamburger');

    if (header.classList.contains('open')) { // Close Hamburger Menu
        body.classList.remove('noscroll');
        header.classList.remove('open');
        fadeElems.forEach(function (element) {
            element.classList.remove('fade-in');
            element.classList.add('fade-out');
        });

    }
    else { // Open Hamburger Menu
        body.classList.add('noscroll');
        header.classList.add('open');
        fadeElems.forEach(function (element) {
            element.classList.remove('fade-out');
            element.classList.add('fade-in');
        });

    }
});