document.getElementById('editCourseBtn').addEventListener('click', function (event) {
    event.preventDefault(); // Prevents the '#' from jumping the page up
    const form = document.getElementById('inlineEditForm');
    form.classList.toggle('d-none'); // Removes or adds the hidden class
});