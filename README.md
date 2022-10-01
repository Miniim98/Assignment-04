# Assignment #4

## C♯

Fork or clone repository.

### Kanban Board part deux

[![Simple-kanban-board-](https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/Simple-kanban-board-.jpg/512px-Simple-kanban-board-.jpg)](https://commons.wikimedia.org/wiki/File:Simple-kanban-board-.jpg "Jeff.lasovski [CC BY-SA 3.0 (https://creativecommons.org/licenses/by-sa/3.0)], via Wikimedia Commons")

Implement and test the `IItemRepository`, `ITagRepository`, and `IUserRepository` interfaces using the rules from [Assignment 03](https://github.com/itu-bdsa/assignment-03/blob/main/README.md#business-rules).

You must use an in-memory database and dependency injection for testing.


## Software Engineering

### Exercise 1

Recapitulate the meaning of _encapsulation_, _inheritance_, and _polymorphism_ in object-oriented programming.
Provide a description of these concepts including UML diagrams to illustrate your descriptions.

### Exercise 2

Draw a UML class diagram that illustrates your implementation of the entities of last week's C♯ assignment, see <https://github.com/itu-bdsa/assignment-03/blob/main/README.md#kanban-board>.
The purpose of the diagram should be to _document_ the main relationships between the entities and their multiplicities.

### Exercise 3

Draw a UML state diagram that illustrates your implementation of the `WorkItem` entity from last week's C♯ assignment, see <https://github.com/itu-bdsa/assignment-03/blob/main/README.md#kanban-board>.
The purpose of the diagram should be to _document_ the different states of the entity and the events that trigger the state changes.

### Exercise 4

For each of the five _SOLID_ design principles, provide an example that illustrates the violation of the specific principle.
Your examples can be given either in code or as UML diagrams.
Briefly explain under which conditions the respective principle is violated.
Note, the examples do not need to be sophisticated.

### Exercise 5

For each of the examples of violations of _SOLID_ design principles in [Exercise 4](./#exercise-4), provide a refactored design that respects the respective design principle.
Again, the refactored designs can be given either in code or as UML diagrams, briefly explain under which conditions the respective principle is not violated any longer, and remember that the examples do not need to be sophisticated.

---

## Submitting the assignment

To submit the assignment you need to create a PDF document using LaTeX that contains the answers to the questions **and** a link to a public GitHub repository that contains a fork of the assignments repository with the completed code.

**Note**: You should not send a PR with your changes.

The PDF file must conform to the following naming convention: `group_<x>_<id1>_<id2>_<id3>_assignment_04.pdf`, where `<x>` is replaced by the number of your group from [README_GROUPS.md](./README_GROUPS.md) and `<id1>`, `<id2>`, and `<id3>` are your respective ITU identifiers. 

You submit via [LearnIT](https://learnit.itu.dk/mod/assign/view.php?id=166021).
