// index.js
const readlineSync = require('readline-sync');
const { connect, client } = require('./db');
const Course = require('./models/Course');
const BinarySearchTree = require('./binaryTree');

async function main() {
  // Connect to the database
  const db = await connect();
  const coursesCollection = db.collection('courses');

  // Load courses from the DB and build the binary search tree
  const coursesData = await coursesCollection.find({}).toArray();
  const bst = new BinarySearchTree();
  coursesData.forEach(courseData => {
    // Assume course documents have fields: id, major, period
    const course = new Course(courseData.id, courseData.major, courseData.period);
    bst.insert(course);
  });

  let exit = false;
  while (!exit) {
    console.log("\n--- CourseAssistant Menu ---");
    console.log("1. View all classes");
    console.log("2. Search for a class by ID");
    console.log("3. Search for classes by major");
    console.log("4. Search for classes by period");
    console.log("5. Exit");

    const choice = readlineSync.question("Enter your choice: ");
    switch (choice) {
      case '1':
        console.log("\nAll classes:");
        const allCourses = bst.inOrder();
        allCourses.forEach(course => {
          console.log(`ID: ${course.id}, Major: ${course.major}, Period: ${course.period}`);
        });
        break;

      case '2':
        const id = readlineSync.question("Enter the class ID: ");
        const foundCourse = bst.search(id);
        if (foundCourse) {
          console.log(`Found: ID: ${foundCourse.id}, Major: ${foundCourse.major}, Period: ${foundCourse.period}`);
        } else {
          console.log("Class not found.");
        }
        break;

      case '3':
        const major = readlineSync.question("Enter the major: ");
        const coursesByMajor = bst.filterByMajor(major);
        if (coursesByMajor.length > 0) {
          coursesByMajor.forEach(course => {
            console.log(`ID: ${course.id}, Major: ${course.major}, Period: ${course.period}`);
          });
        } else {
          console.log("No classes found for that major.");
        }
        break;

      case '4':
        const period = readlineSync.question("Enter the period: ");
        const coursesByPeriod = bst.filterByPeriod(period);
        if (coursesByPeriod.length > 0) {
          coursesByPeriod.forEach(course => {
            console.log(`ID: ${course.id}, Major: ${course.major}, Period: ${course.period}`);
          });
        } else {
          console.log("No classes found for that period.");
        }
        break;

      case '5':
        exit = true;
        break;

      default:
        console.log("Invalid choice. Please try again.");
    }
  }

  // Close the database connection
  await client.close();
}

main();
