# The script of the game goes in this file.

# Declare characters used by this game. The color argument colorizes the
# name of the character.

define e = Character("Eileen")


# The game starts here.

label start:

    # Show a background. This uses a placeholder by default, but you can
    # add a file (named either "bg room.png" or "bg room.jpg") to the
    # images directory to show it.

    scene black

    # This shows a character sprite. A placeholder is used, but you can
    # replace it by adding a file named "eileen happy.png" to the images
    # directory.

    show eileen happy

    # These display lines of dialogue.

    e "Youve created a new RenPy game"

    e "Once you add a story, pictures, and music, you can release it to the world!"

    menu:
        "This is option 1":
            "Option 1 leads to Stars"
        "This is option 2":
            "Option 2 leads to Black hole{w=1.0} not that one"
        "This is option 3":
            "Option 3 leads to Death{w=1.0}of Stars"
        "This is option 4":
            "Option 4 leads to the legendary image called eileen happy.png"

    "Are you alive? {w=1.0}Hmmmmmm"

    # This ends the game.

    return
