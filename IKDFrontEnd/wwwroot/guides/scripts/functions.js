
///* -------------------------------
//	* Responsive Menu
//---------------------------------- */
//// Add remove Active class

var selector = 'ul.yes-no li a';
$(selector).on('click', function(){
$(selector).removeClass('active');
var box=$(this).attr("data-box");
var parent=this.parentNode.parentNode.parentNode;
var childs=parent.getElementsByTagName("div");
for(let i=0;i<childs.length;i++)
{
  if(childs[i].id===box)
    childs[i].style.display="block"
  else
    childs[i].style.display="none"
}
$(this).addClass('active');
});




/* -------------------------------
 * Responsive Menu
---------------------------------- */
// Add/remove Active class

//var selector = document.querySelectorAll('ul.yes-no li a');

//selector.forEach(function (link) {
//    link.addEventListener('click', function (e) {
//        e.preventDefault();

//        // Remove active class from all links
//        selector.forEach(function (l) {
//            l.classList.remove('active');
//        });

//        var box = this.getAttribute("data-box");
//        var parent = this.closest('ul').parentNode; // find the grandparent of ul
//        var childs = parent.getElementsByTagName("div");

//        for (let i = 0; i < childs.length; i++) {
//            if (childs[i].id === box) {
//                childs[i].style.display = "block";
//            } else {
//                childs[i].style.display = "none";
//            }
//        }

//        // Add active class to clicked link
//        this.classList.add('active');
//    });
//});
