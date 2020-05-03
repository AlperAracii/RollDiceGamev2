/*
GAME RULES:

- The game has 2 players, playing in rounds
- In each turn, a player rolls a dice as many times as he whishes. Each result get added to his ROUND score
- BUT, if the player rolls a 1, all his ROUND score gets lost. After that, it's the next player's turn
- The player can choose to 'Hold', which means that his ROUND score gets added to his GLBAL score. After that, it's the next player's turn
- The first player to reach 100 points (10 points for easy example) on GLOBAL score wins the game

*/

var scores = [0,0], roundScore = 0, activePlayer = 0;
var dice = 0;
var sum;
// var diceImage = document.getElementById('dice').src;
console.log(dice);

document.getElementById('btnNew').onclick = function () { newGameFunc() };
document.getElementById('btnRoll').onclick = function() {rollDiceFunc()};
document.getElementById('btnHold').onclick = function() {holdFunc()};


function rollDiceFunc() {

    document.querySelector('.dice').classList.add("rollAnimation");
    dice = Math.floor( (Math.random() * 6) + 1);    
    document.getElementById('dice').src = "../images/dice-" + dice + ".png";
    
    if(dice !== 1){
        roundScore += dice;
        document.getElementById('current-' + activePlayer).textContent = roundScore;
        sum = scores[activePlayer] + roundScore;

        if(sum >= 10){
            document.getElementById('score-' + activePlayer).textContent = sum;
            document.getElementById('btnOver').style.display = "block";
            document.getElementById('btnScore').style.display = "block";
            document.getElementById('btnHold').style.display = "none";
            document.getElementById('btnRoll').style.display = "none";

            var score0 = $("#score-0").html();
            var score1 = $("#score-1").html();
            $.ajax({
                type: "POST",
                url: "/Home/SaveResult",
                data: { score0: score0, score1: score1 },
                dataType : "application/json"
            });
        }

    } else {
        roundScore = 0;
        holdFunc();
    }
    document.querySelector('.dice').classList.remove("rollAnimation");
}

function holdFunc() {

    if(activePlayer == 0){
        document.querySelector('.player-0-panel').classList.remove("active");
        document.querySelector('.player-1-panel').classList.add("active");
    } else {
        document.querySelector('.player-1-panel').classList.remove("active");
        document.querySelector('.player-0-panel').classList.add("active");
    }
    

    document.getElementById('current-' + activePlayer).textContent = 0;

    scores[activePlayer] += roundScore;
    document.getElementById('score-' + activePlayer).textContent = scores[activePlayer];    
    activePlayer == 0 ? activePlayer = 1 : activePlayer = 0;
    roundScore = 0;
    // console.log(activePlayer)
}

function newGameFunc() {
    window.confirm("Are You Sure Want New Game?")
    window.location.replace("https://localhost:44364/Home/Login");
}