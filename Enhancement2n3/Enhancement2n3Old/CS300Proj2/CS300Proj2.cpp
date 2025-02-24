//============================================================================
// Name        : CS300Proj2.cpp
// Author      : Matthew Cohen
// Version     : 1.0
// Copyright   : Copyright © 2023 SNHU COCE
// Description : Project 2 of CS300
//============================================================================

// CS300Proj2.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include <algorithm>

using namespace std;

struct Course {
    string courseNumber;
    string name;
    vector<string> prerequisites;
    Course* left;
    Course* right;

    Course() : left(nullptr), right(nullptr) {}
    Course(string courseNum, string courseName) : courseNumber(courseNum), name(courseName), left(nullptr), right(nullptr) {}
};

class BinarySearchTree {
private:
    Course* root;

    void addNode(Course*& node, Course* newCourse) {
        if (node == nullptr) {
            node = newCourse;
        }
        else if (newCourse->courseNumber < node->courseNumber) {
            addNode(node->left, newCourse);
        }
        else {
            addNode(node->right, newCourse);
        }
    }

    void inOrder(Course* node) {
        if (node != nullptr) {
            inOrder(node->left);
            cout << node->courseNumber << ": " << node->name << endl;
            inOrder(node->right);
        }
    }

    Course* findCourse(Course* node, const string& courseNumber) {
        if (node == nullptr || node->courseNumber == courseNumber) {
            return node;
        }
        if (courseNumber < node->courseNumber) {
            return findCourse(node->left, courseNumber);
        }
        else {
            return findCourse(node->right, courseNumber);
        }
    }

public:
    BinarySearchTree() : root(nullptr) {}

    void insert(Course* newCourse) {
        addNode(root, newCourse);
    }

    void printInOrder() {
        inOrder(root);
    }

    Course* search(const string& courseNumber) {
        return findCourse(root, courseNumber);
    }
};

vector<string> split(const string& s, char delimiter) {
    vector<string> tokens;
    string token;
    istringstream tokenStream(s);
    while (getline(tokenStream, token, delimiter)) {
        tokens.push_back(token);
    }
    return tokens;
}

bool loadCourses(const string& filename, BinarySearchTree& courses) {
    ifstream file(filename);
    if (!file.is_open()) {
        cerr << "Error: Unable to open file." << endl;
        return false;
    }

    string line;
    while (getline(file, line)) {
        vector<string> tokens = split(line, ',');
        if (tokens.size() < 2) {
            cerr << "Error: Invalid line format." << endl;
            continue;
        }
        string courseNumber = tokens[0];
        string name = tokens[1];
        Course* course = new Course(courseNumber, name);
        for (size_t i = 2; i < tokens.size(); ++i) {
            course->prerequisites.push_back(tokens[i]);
        }
        courses.insert(course);
    }
    file.close();
    return true;
}

void printCourseDetails(BinarySearchTree& courses, const string& courseNumber) {
    Course* course = courses.search(courseNumber);
    if (course != nullptr) {
        cout << "Course Number: " << course->courseNumber << endl;
        cout << "Course Name: " << course->name << endl;
        if (!course->prerequisites.empty()) {
            cout << "Prerequisites: ";
            for (const string& prereq : course->prerequisites) {
                cout << prereq << " ";
            }
            cout << endl;
        }
        else {
            cout << "No prerequisites." << endl;
        }
    }
    else {
        cout << "Course " << courseNumber << " not found." << endl;
    }
}

int main() {
    BinarySearchTree courses;
    while (true) {
        cout << "What would you like to do?" << endl;
        cout << "Menu:" << endl;
        cout << "1. Load course data" << endl;
        cout << "2. Print all courses" << endl;
        cout << "3. Print course details" << endl;
        cout << "9. Exit" << endl;
        int choice;
        cin >> choice;
        cin.ignore();

        if (choice == 1) {
            string filename;
            cout << "Enter the file name: ";
            getline(cin, filename);
            if (loadCourses(filename, courses)) {
                cout << "Courses loaded." << endl;
            }
        }
        else if (choice == 2) {
            courses.printInOrder();
        }
        else if (choice == 3) {
            string courseNumber;
            cout << "Enter course number: ";
            getline(cin, courseNumber);
            printCourseDetails(courses, courseNumber);
        }
        else if (choice == 9) {
            cout << "Exiting program." << endl;
            break;
        }
        else {
            cout << "Invalid input. Try again." << endl;
        }
    }
    return 0;
}




