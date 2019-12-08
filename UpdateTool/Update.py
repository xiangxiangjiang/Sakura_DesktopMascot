import requests
# import time
import json
from sys import argv
from bs4 import BeautifulSoup


# 首先我们写好抓取网页的函数
def get_html(url):
    try:
        headers = {"Host": "github.com",
                   "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36"}
        r = requests.get(url, timeout=30, headers=headers)
        r.raise_for_status()
        # print(r.status_code)
        # 这里我们知道百度贴吧的编码是utf-8，所以手动设置的。爬去其他的页面时建议使用：
        # r.endcodding = r.apparent_endconding
        r.encoding = 'utf-8'
        return r.text
    except:
        return None


def get_GithubData(url):

    posts = {}
    html = get_html(url)
    if html == None:
        print('网络错误，请确保能够访问GitHub!\n')
        return None

    soup = BeautifulSoup(html, 'lxml')
    newUrl = ''
    try:
        div = soup.find(
            'div', class_='d-flex flex-items-center')
        string = div.find('a')['data-hydro-click'] + ''
        newUrl = (json.loads(string))['payload']['originating_url']
    except:
        print('获取重定向网址失败!\n')
        return None
    # 开始重定向到新网址
    print('重定向到：' + newUrl + '\n')
    html = get_html(newUrl)
    if html == None:
        print('网络错误，请确保能够访问GitHub!\n')
        return None
    soup = BeautifulSoup(html, 'lxml')

    try:
        release = soup.find('div', class_='border-top')
        posts['ver'] = release.find(
            'a', class_='muted-link css-truncate')['title']
        print('最新版本：' + posts['ver'] + '\n')

        posts['log'] = release.find(
            'div', class_='markdown-body').find('p').text
        print('更新日志：' + posts['log'] + '\n')

        posts['time'] = release.find('relative-time').text
        print('更新时间：' + posts['time'] + '\n')

        posts['link'] = 'https://github.com/' + release.find(
            'a', class_='d-flex flex-items-center min-width-0')['href']
        print('下载地址：' + posts['link'] + '\n')

    except:
        print('Github数据筛选失败!\n')
        return None
    return posts


out = {0: '>按回车退出<\n\n', 1: '>按回车重试<\n\n', 2: '>按回车开始更新<\n\n'}


def Output(code):
    input(out.get(code, '无此消息!\n'))


if __name__ == "__main__":
    exePath, ver = '', ''
    if not argv:
        exePath, ver = argv
    print('\t-==八重樱桌宠==-\n\n')
    if exePath != '':
        print('当前版本：' + ver + '\n' + '运行目录：' + exePath + '\n')
    print('若速度过慢可手动前往码云下载：\n\thttps://gitee.com/JasonMa233/Sakura_DesktopMascot/releases\n\n')
    while True:
        print('检查更新中……\n\n')
        githubData = get_GithubData(
            'https://github.com/Jason-Ma-233/Sakura_DesktopMascot/releases/latest')
        if githubData != None:
            Output(2)
            # 下载压缩包
            # 解压
            # 结束程序，复制并覆盖文件
            # 删除temp文件夹
            break
        else:
            Output(1)
            continue
